# MYA Backend

Backend .NET 10 per autenticazione cliente, MFA via SMS/email e generazione token del nuovo progetto MYA.

## Stack

- .NET 10 / ASP.NET Core Web API
- Solution multi-project: `MYA.Api`, `MYA.Business`, `MYA.Data`, `MYA.Models`
- SQL Server con `Microsoft.Data.SqlClient` `7.0.2`
- Invio email MFA tramite API HTTP `AuthNew/SendMail`
- JWT con `System.IdentityModel.Tokens.Jwt` `8.22.0`
- Swagger UI con `Swashbuckle.AspNetCore` `10.2.3`
- NuGet Audit abilitato su dipendenze dirette e transitive

## Architettura

- `MYA.Api`: startup, DI, CORS, Swagger e controller HTTP.
- `MYA.Business`: regole applicative di login, MFA, crypto legacy e token.
- `MYA.Data`: DB helper async, connection factory e repository per stored procedure.
- `MYA.Models`: request, response, DTO e options di configurazione.
- `database`: script SQL versionati per Puzzle e SAT.

## Flusso Login

```text
POST /api/myAuth/login
-> valida codice cliente, login e password su Puzzle
-> password cifrata con algoritmo legacy compatibile
-> richiede utente con Wus_Id_App = 2
-> genera codice MFA a 6 cifre
-> aggiorna T_WEB_UTENTI.Wus_2FA_Code e Wus_2FA_LastRequest
-> se Wus_2FA_NumCell e' presente scrive SMS su SAT Macro_Vpn tramite wrapper nuova
-> se Wus_2FA_NumCell manca invia email a Wus_Email tramite API SendMail
-> se mancano sia cellulare sia email genera token e ritorna un avviso sicurezza
-> ritorna userId, login, nome, canale MFA, destinazione mascherata, scadenza MFA o token diretto
```

```text
POST /api/myAuth/verifyMfa
-> verifica codice MFA e scadenza
-> azzera Wus_2FA_Code dopo successo
-> genera access token JWT e refresh token
-> scrive JWTokens con idApp = 2 e permission = 0
-> ritorna token al frontend
```

## Endpoint

Rotte camelCase, senza trattini:

- `POST /api/myAuth/login`
- `POST /api/myAuth/verifyMfa`

Swagger:

```text
http://127.0.0.1:5110/swagger
```

## JWT

Il JWT nuovo non contiene permessi SAT o `userFunctionSat`.

Claim applicativi previsti:

- `sub`: id riga utente/token
- `jti`: id univoco token
- `idUtente`: id utente funzionale eventuale
- `email`: email utente, se presente
- `ragioneSociale`: ragione sociale cliente, se presente

La colonna `JWTokens.permission` resta valorizzata a `0` solo per compatibilita' con lo schema legacy.

## Configurazione Locale

Non committare segreti in `appsettings.json`. Usare variabili ambiente:

```powershell
$env:Database__PuzzleConnectionString="Server=10.20.0.80;Database=smartsat_co;User Id=...;Password=...;Encrypt=False;TrustServerCertificate=True"
$env:Database__SatConnectionString="Server=10.20.0.30;Database=smartsat_co;User Id=...;Password=...;Encrypt=False;TrustServerCertificate=True"
$env:MyAuth__LegacyPasswordKeyPhrase="..."
$env:MyAuth__MissingMfaSecurityNotice="Account sprovvisto di sicurezza a due fattori..."
$env:Jwt__SigningKey="chiave-lunga-almeno-32-byte"
$env:MailApi__BearerToken="..."
```

## Comandi

```powershell
cd C:\MYA\BE
dotnet restore
dotnet build .\MYA.slnx
dotnet run --project .\src\MYA.Api\MYA.Api.csproj --launch-profile http
```

## Sicurezza Pacchetti

`Directory.Build.props` abilita NuGet Audit in modalita' transitive.
Le vulnerabilita' `NU1901`, `NU1902`, `NU1903` e `NU1904` sono trattate come errori.

Controllo manuale:

```powershell
dotnet list .\MYA.slnx package --vulnerable --include-transitive
```

## Stored Procedure Nuove

Puzzle `10.20.0.80`:

```text
database/Puzzle/001_sp_My_GetLogin.sql
database/Puzzle/002_sp_My_SetMfaCode.sql
database/Puzzle/003_sp_My_VerifyMfaCode.sql
database/Puzzle/004_sp_My_InsertJWTokenValue.sql
database/Puzzle/005_sp_My_GetJWTokenValue.sql
database/Puzzle/006_seed_luca_pesola_mfa_email.sql
```

SAT `10.20.0.30`:

```text
database/Sat/001_sp_My_EnqueueMfaSms.sql
```

`sp_My_EnqueueMfaSms` e' una wrapper nuova che chiama la stored legacy `dbo.sp_Insert_PZ_MacroVpn_Dispatcher_SmsEmail`.

## MFA Email

Se l'utente ha `Wus_2FA_NumCell` vuoto ma `Wus_Email` valorizzato, il backend invia il codice MFA via HTTP POST a:

```text
https://devapi.axitea.com/api/AuthNew/SendMail
```

Payload:

```json
{
  "To": "giulio.caruso%40axitea.com",
  "Subject": "Codice di sicurezza MYA",
  "Body": "Il tuo codice di sicurezza e' 123456. Non condividerlo.",
  "DescrSender": "PUZZLE",
  "HasCC": false,
  "CC": "",
  "BCC": ""
}
```

L'header `Authorization: Bearer ...` viene letto da `MailApi__BearerToken` e non deve essere salvato nel repository.

## MFA Mancante

Se l'utente non ha ne' `Wus_2FA_NumCell` ne' `Wus_Email`, il backend non blocca l'accesso:

- registra un warning applicativo;
- genera subito access token e refresh token;
- ritorna `requiresMfa = false`, `token` e `securityNotice`;
- il frontend mostra un popup dedicato che segnala account sprovvisto di sicurezza a due fattori.
