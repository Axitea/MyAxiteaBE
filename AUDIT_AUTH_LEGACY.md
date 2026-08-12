# Audit Auth Legacy ApiAxitea

Data audit: 2026-08-05

## Obiettivo

Ricostruire il flusso di autenticazione legacy `ApiAxitea` / `AuthNewController` per poter creare il nuovo backend ASP.NET Core `net10.0` mantenendo compatibilita con gli utenti esistenti.

## Sorgenti Analizzate

- `C:\DEV\ApiAxitea\ApiAxitea\ApiAxitea\Controllers\NewSat\AuthNewController.cs`
- `C:\DEV\AxiDataLib\SAT\API\DaoUtils.cs`
- `C:\DEV\AxiDataLib\SAT\API\DaoNewSat.cs`
- `C:\DEV\AxiUtilitiesLib\Security\StringCipher.cs`
- `C:\DEV\ApiAxitea\ApiAxitea\ApiAxitea\Utilities\TokenManager.cs`
- database `smartsat_co` su server DEV `10.20.0.80`

Le configurazioni sensibili presenti nel legacy sono state lette solo per audit e non sono riportate in questo documento.

## Flusso Legacy End-To-End

Endpoint legacy:

```text
POST api/AuthNew/GetLogin
```

Payload:

```json
{
  "codiceCliente": "...",
  "username": "...",
  "password": "...",
  "isMobile": false
}
```

Passaggi:

1. `AuthNewController.GetLogin` valida solo che `codiceCliente`, `username` e `password` non siano null.
2. Chiama `DaoUtils.GetLogin(authInfo)`.
3. Chiama `DaoUtils.GetLoginClaims(authInfo)`.
4. Se l'utente esiste, genera JWT con `JwtManager.GenerateToken`.
5. Genera refresh token random.
6. Salva JWT e refresh token su tabella `JWTokens` tramite `DaoNewSat.InsertJWTokenValue`.
7. Restituisce `GetLoginResponse` con `userInfo` e `token`.
8. Scrive cookie `refresh-token`.

## Query Login Reale

Il codice C# non interroga direttamente `T_WEB_UTENTI`: usa la stored procedure `dbo.sp_GetLogin`.

Definizione recuperata dal DB:

```sql
CREATE PROCEDURE [dbo].[sp_GetLogin]
  @CodCliente varchar(50),
  @Login varchar(255),
  @Password varchar(255)
AS
BEGIN
  SELECT
    Wus_IdUtente as Wus_IdUtenteToken,
    ISNULL(Wus_IdDipendente, Wus_IdUtente) as Wus_IdUtente,
    Wus_Nome,
    Wus_Permessi,
    Wus_Email,
    Wus_CliRagSociale,
    Wus_IdScenario,
    Wus_Lingua
  FROM T_WEB_UTENTI
  WHERE Wus_CodCliente = @CodCliente
    AND Wus_Login = @Login
    AND Wus_NewSatPassword = @Password
END
```

Punti chiave:

- Il campo password usato dal nuovo SAT e' `Wus_NewSatPassword`.
- `Wus_Password` esiste ma appartiene alla parte storica/legacy precedente.
- `Wus_IdUtenteToken` non e' una colonna fisica: e' alias di `Wus_IdUtente`.
- `Wus_IdUtente` restituito all'app e' `ISNULL(Wus_IdDipendente, Wus_IdUtente)`.

## Colonne Rilevanti

| Colonna | Tipo DB | Note |
| --- | --- | --- |
| `Wus_IdUtente` | `bigint` | ID utente originale, usato anche come token id alias |
| `Wus_IdDipendente` | `int null` | se presente sostituisce `Wus_IdUtente` nel claim operativo |
| `Wus_CodCliente` | `varchar(50)` | codice cliente |
| `Wus_Login` | `varchar(255)` | login utente |
| `Wus_Password` | `varchar(255)` | password storica, non usata da `AuthNew` |
| `Wus_NewSatPassword` | `nvarchar(255)` | password cifrata usata dal login nuovo |
| `Wus_Permessi` | `int` | permission principale |
| `Wus_Email` | `varchar(255)` | email |
| `Wus_CliRagSociale` | `varchar(255) null` | ragione sociale |
| `Wus_IdScenario` | `int null` | default legacy lato C#: `1` se vuoto |
| `Wus_Lingua` | `nvarchar(5) null` | default legacy lato C#: `it` se vuoto |
| `Wus_ChangePwd` | `nchar(1) null` | cambio password |

## Password Crypto

Legacy:

```csharp
string pwdEcnc = StringCipher.EncryptString(KEY_PHRASE, authInfo.password);
```

Algoritmo `StringCipher`:

- AES
- chiave: bytes UTF-8 della `KEY_PHRASE`
- IV: 16 byte tutti a zero
- mode/padding: default AES .NET, quindi CBC + PKCS7
- output: Base64
- cifratura deterministica: stessa password + stessa chiave producono sempre lo stesso valore

Compatibilita .NET 10:

- Verificata con probe temporaneo `net10.0`.
- Un ciphertext legacy e' stato decifrato e ricifrato identico.
- Risultato: compatibilita byte-per-byte mantenibile nel nuovo backend.

Nota di sicurezza:

- Questo schema e' reversibile e usa IV fisso.
- Per compatibilita con utenti esistenti va mantenuto nella prima fase.
- Per una fase futura conviene prevedere migrazione progressiva verso hashing password moderno, ad esempio PBKDF2/Argon2/bcrypt, dopo login riuscito.

## Refresh Token

Endpoint legacy:

```text
GET api/AuthNew/RefreshToken
```

Flusso:

1. Legge cookie `refresh-token`.
2. Cerca token su `JWTokens` con `dbo.sp_GetJWTokenValue`.
3. Estrae da `jwt.Username` la prima parte prima di `#`.
4. Richiama `DaoUtils.GetLoginClaims(idUserForRefresh)`.
5. Genera nuovo JWT.
6. Genera e salva nuovo refresh token.
7. Scrive nuovo cookie.

Stored procedure reali recuperate dal DB:

```sql
CREATE PROCEDURE dbo.sp_InsertJWTokenValue
  @TokenValue nvarchar(max),
  @Username nvarchar(30),
  @Permission int,
  @RefreshToken nvarchar(200)
AS
BEGIN
  INSERT INTO JWTokens (tokenValue, username, dateToken, permission, refreshToken)
  VALUES (@TokenValue, @Username, CURRENT_TIMESTAMP, @Permission, @RefreshToken)
END
```

```sql
CREATE PROCEDURE dbo.sp_GetJWTokenValue
  @RefreshToken nvarchar(200)
AS
BEGIN
  SELECT * FROM JWTokens WHERE RefreshToken = @RefreshToken
END
```

## Decisione Per Nuovo BE

Per mantenere compatibilita:

1. Nuovo progetto ASP.NET Core `net10.0`.
2. Controller iniziale `MyAuthController`.
3. Endpoint equivalente a login, ad esempio `POST /api/my-auth/login`.
4. Usare `Microsoft.Data.SqlClient`.
5. Connection string in `appsettings.Development.json` o secret manager, non committata.
6. Implementare `LegacyPasswordCipher` compatibile con `StringCipher`.
7. Login con stored procedure `dbo.sp_GetLogin`, oppure query parametrizzata equivalente.
8. Usare `Wus_NewSatPassword`, non `Wus_Password`.
9. Risposta iniziale minimale compatibile con FE, poi evolvibile.

## Rischi Da Gestire Nel Porting

- Non copiare segreti dal `Web.config` legacy dentro repository Git.
- Non usare query interpolate: solo parametri SQL.
- Gestire `Wus_IdUtente` DB come `long`; nel legacy viene convertito a `int`, ma il DB e' `bigint`.
- Replicare esattamente AES legacy solo per compatibilita.
- Separare crypto legacy da futura strategia password moderna.
- Non affidarsi agli script SQL locali per i token: almeno `sp_Tokens.sql` risulta piu vecchio della stored reale sul DB.

## Conclusione

La compatibilita con gli account esistenti e' fattibile in .NET 10.

La parte da preservare fedelmente e':

```text
password in chiaro ricevuta dal FE
-> AES legacy con KEY_PHRASE
-> confronto con T_WEB_UTENTI.Wus_NewSatPassword
```

Il nuovo BE puo nascere pulito, ma deve isolare questa compatibilita in un servizio dedicato, cosi in futuro si potra migrare la sicurezza password senza riscrivere controller e flusso auth.
