# Documentazione BE - MYA

## Obiettivo

Il backend MYA gestisce il nuovo flusso di autenticazione cliente:

```text
login credenziali
-> MFA via SMS o email
-> verifica OTP
-> generazione JWT e refresh token
```

Il progetto mantiene compatibilita' con dati e crypto legacy, ma introduce stored procedure nuove con prefisso `sp_My_` per non rompere il pregresso.

## Stack Tecnico

| Area | Tecnologia |
| --- | --- |
| Runtime | .NET 10 |
| API | ASP.NET Core Web API |
| Linguaggio | C# |
| Database | SQL Server |
| Driver SQL | `Microsoft.Data.SqlClient` `7.0.2` |
| Invio mail | HTTP POST verso `AuthNew/SendMail` |
| Token | JWT con `System.IdentityModel.Tokens.Jwt` `8.22.0` |
| Swagger | `Swashbuckle.AspNetCore` `10.2.3` |
| Sicurezza pacchetti | NuGet Audit con vulnerabilita' trattate come errori |

## Solution

```text
BE/
+-- MYA.slnx
+-- Directory.Build.props
+-- MYA.Api
+-- MYA.Business
+-- MYA.Data
+-- MYA.Models
+-- database/
|   +-- Puzzle
|   +-- Sat
+-- AUDIT_AUTH_LEGACY.md
```

## Progetti

| Progetto | Responsabilita' |
| --- | --- |
| `MYA.Api` | Controllers HTTP, Swagger, CORS, configurazione, dependency injection |
| `MYA.Business` | Servizi concreti per autenticazione, MFA, crypto legacy, token e clienti |
| `MYA.Data` | Data access SQL organizzato per database, connection factory e executor comuni |
| `MYA.Models` | DTO, request/response, options e record condivisi |

I controller sono raccolti nella sola cartella `MYA.Api/Controllers`. Le dipendenze interne
usano classi concrete: non sono presenti interfacce per classi con una sola implementazione.

`MYA.Data` e' organizzato per database:

| Cartella | Classe | Responsabilita' |
| --- | --- | --- |
| `Common` | `SqlConnectionFactory`, `SqlExecutor` | Connessioni, timeout, parametri ed esecuzione stored |
| `Puzzle` | `PuzzleDataAccess` | Login, MFA e token su Puzzle |
| `Sat` | `SatDataAccess` | Accodamento SMS MFA su SAT |
| `DbUnico` | `DbUnicoDataAccess` | Clienti e contratti su DBUNICO |

## Configurazione

Le opzioni vengono lette da `appsettings.json` e sovrascritte tramite variabili ambiente.

```text
Database__PuzzleConnectionString
Database__SatConnectionString
Database__DBUNICOConnectionString
Database__CommandTimeoutSeconds
MyAuth__AppId
MyAuth__LegacyPasswordKeyPhrase
MyAuth__MfaCodeLength
MyAuth__MfaTtlMinutes
MyAuth__SmsTextTemplate
MyAuth__MailSubject
MyAuth__MailTextTemplate
MyAuth__MailSenderDescription
MyAuth__MissingMfaSecurityNotice
MailApi__SendMailUrl
MailApi__BearerToken
MailApi__TimeoutSeconds
MailApi__UrlEncodeRecipient
Jwt__Issuer
Jwt__Audience
Jwt__SigningKey
Jwt__AccessTokenMinutes
Jwt__RefreshTokenDays
```

I segreti non devono essere salvati nel repository.

## Endpoint HTTP

### POST `/api/myAuth/login`

Avvia login e MFA.

Request:

```json
{
  "codiceCliente": "8888888888",
  "login": "giulio.caruso",
  "password": "***"
}
```

Response OK:

```json
{
  "success": true,
  "data": {
    "userId": 4022,
    "login": "giulio.caruso",
    "displayName": "Giulio Caruso",
    "maskedPhoneNumber": "******0833",
    "mfaDeliveryChannel": "sms",
    "maskedDestination": "******0833",
    "mfaExpiresAt": "...",
    "requiresMfa": true,
    "token": null,
    "securityNotice": null
  },
  "message": null
}
```

Effetti:

- cifra la password con algoritmo legacy;
- chiama `dbo.sp_My_GetLogin` su Puzzle;
- genera codice MFA a 6 cifre;
- chiama `dbo.sp_My_SetMfaCode` su Puzzle;
- se `Wus_2FA_NumCell` e' presente chiama `dbo.sp_My_EnqueueMfaSms` su SAT;
- se `Wus_2FA_NumCell` manca invia email a `Wus_Email` tramite API `SendMail`;
- se mancano sia cellulare sia email genera subito il token e ritorna `requiresMfa = false` con `securityNotice`.

### POST `/api/myAuth/verifyMfa`

Verifica OTP e rilascia token.

Request:

```json
{
  "userId": 4022,
  "code": "123456"
}
```

Response OK:

```json
{
  "success": true,
  "data": {
    "accessToken": "...",
    "refreshToken": "...",
    "accessTokenExpiresAt": "...",
    "userId": 4022,
    "login": "giulio.caruso",
    "appId": 2
  },
  "message": null
}
```

Effetti:

- chiama `dbo.sp_My_VerifyMfaCode` su Puzzle;
- azzera `Wus_2FA_Code` dopo verifica positiva;
- genera access token JWT;
- genera refresh token;
- chiama `dbo.sp_My_InsertJWTokenValue`;
- scrive su `JWTokens` con `idApp = 2` e `permission = 0`.

### GET `/api/MyClienti/GetAllContrattiByCliente`

Riceve il parametro query `CodCliente` e restituisce contratti, posizioni e tipologie di
servizio aggregate per indirizzo tramite DBUNICO. La response esistente con campi `Response`,
`Data` e `Message` resta invariata.

### GET `/api/MyClienti/GetAllClienti`

Restituisce i clienti disponibili su DBUNICO. Anche per questo endpoint la response esistente
resta invariata.

## JWT

Il JWT nuovo non contiene permessi legacy SAT.

Claim previsti:

| Claim | Significato |
| --- | --- |
| `sub` | Id riga utente/token |
| `jti` | Identificativo univoco token |
| `idUtente` | Id utente funzionale |
| `email` | Email utente, se presente |
| `ragioneSociale` | Ragione sociale cliente, se presente |

Non sono presenti:

- `permission`
- permessi `userFunctionSat`
- autorizzazioni legacy SAT

La tabella `JWTokens` conserva la colonna `permission`; il nuovo progetto la valorizza a `0` solo per compatibilita' schema.

## Metodi Principali

### API

| File | Metodo | Descrizione |
| --- | --- | --- |
| `MYA.Api/Controllers/MyAuthController.cs` | `Login` | Espone `POST /api/myAuth/login` |
| `MYA.Api/Controllers/MyAuthController.cs` | `VerifyMfa` | Espone `POST /api/myAuth/verifyMfa` |
| `MYA.Api/Controllers/MyClientiController.cs` | `GetAllContratti` | Espone `GET /api/MyClienti/GetAllContrattiByCliente` |
| `MYA.Api/Controllers/MyClientiController.cs` | `GetAllClienti` | Espone `GET /api/MyClienti/GetAllClienti` |

### Business

| File | Metodo | Descrizione |
| --- | --- | --- |
| `AuthService.cs` | `StartLoginAsync` | Orchestrazione login, MFA code e invio SMS/email |
| `AuthService.cs` | `VerifyMfaAsync` | Verifica OTP e token |
| `AuthService.cs` | `IssueTokenAsync` | Emissione token condivisa fra verifica MFA e accesso senza canali 2FA |
| `MfaMailApiClient.cs` | `SendMfaCodeAsync` | Invio codice MFA via API HTTP `SendMail` |
| `LegacyPasswordCipher.cs` | `Encrypt` | Cifratura password compatibile legacy |
| `LegacyPasswordCipher.cs` | `Decrypt` | Decifratura compatibile legacy |
| `MfaCodeGenerator.cs` | `Generate` | Generazione codice numerico sicuro |
| `JwtTokenService.cs` | `Issue` | Creazione access token e refresh token |
| `ClientiService.cs` | `GetAllContrattiClienteAsync` | Coordina il recupero contratti per cliente |
| `ClientiService.cs` | `GetClientiAsync` | Coordina il recupero clienti |

### Data

| File | Metodo | Descrizione |
| --- | --- | --- |
| `Common/SqlConnectionFactory.cs` | `Create` | Crea connessione Puzzle, SAT o DBUNICO |
| `Common/SqlExecutor.cs` | `QueryAsync` | Esegue stored e mappa piu' righe |
| `Common/SqlExecutor.cs` | `QuerySingleOrDefaultAsync` | Esegue stored e mappa una riga |
| `Common/SqlExecutor.cs` | `ExecuteAsync` | Esegue stored senza result set |
| `Puzzle/PuzzleDataAccess.cs` | `GetLoginAsync` | Chiama `sp_My_GetLogin` |
| `Puzzle/PuzzleDataAccess.cs` | `SaveMfaCodeAsync` | Chiama `sp_My_SetMfaCode` |
| `Puzzle/PuzzleDataAccess.cs` | `VerifyMfaCodeAsync` | Chiama `sp_My_VerifyMfaCode` |
| `Puzzle/PuzzleDataAccess.cs` | `SaveIssuedTokenAsync` | Chiama `sp_My_InsertJWTokenValue` |
| `Puzzle/PuzzleDataAccess.cs` | `GetTokenByRefreshTokenAsync` | Chiama `sp_My_GetJWTokenValue` |
| `Sat/SatDataAccess.cs` | `EnqueueMfaSmsAsync` | Chiama `sp_My_EnqueueMfaSms` |
| `DbUnico/DbUnicoDataAccess.cs` | `GetAllClientiAsync` | Chiama `sp_GetALL_MY_ViewDBUnico` |
| `DbUnico/DbUnicoDataAccess.cs` | `GetContrattiByClienteAsync` | Chiama `sp_Get_CordinateCliente_byCodCliente` |

## Stored Procedure

### Puzzle `10.20.0.80`

| Stored | Scopo |
| --- | --- |
| `dbo.sp_My_GetLogin` | Login credenziali con `Wus_Id_App = 2` |
| `dbo.sp_My_SetMfaCode` | Scrive codice MFA, data richiesta |
| `dbo.sp_My_VerifyMfaCode` | Verifica OTP e scadenza, poi azzera codice |
| `dbo.sp_My_InsertJWTokenValue` | Scrive token su `JWTokens` |
| `dbo.sp_My_GetJWTokenValue` | Legge token da refresh token e `idApp` |
| `006_seed_luca_pesola_mfa_email.sql` | Seed insert-only utente test `luca.pesola` sulla coppia cliente/login/app |

### SAT `10.20.0.30`

| Stored | Scopo |
| --- | --- |
| `dbo.sp_My_EnqueueMfaSms` | Wrapper nuova per accodare SMS MFA su MacroVPN |

`sp_My_EnqueueMfaSms` chiama la stored legacy `dbo.sp_Insert_PZ_MacroVpn_Dispatcher_SmsEmail`.

## Sicurezza

- NuGet Audit attivo in `Directory.Build.props`.
- Vulnerabilita' `NU1901-NU1904` trattate come errori.
- Nessun segreto nel repository.
- Password confrontata in forma cifrata legacy.
- MFA code generato con `RandomNumberGenerator`.
- MFA inviato via SMS quando e' presente `Wus_2FA_NumCell`; fallback via email quando il numero manca e `Wus_Email` e' valorizzato.
- Se mancano entrambi i canali, l'accesso viene consentito ma tracciato nei log applicativi con warning e mostrato nel FE con popup di sicurezza.
- Bearer token della Mail API configurato tramite variabile ambiente `MailApi__BearerToken`.
- Token signing key minima 32 byte.

## Comandi

```powershell
cd C:\MYA\BE
dotnet restore
dotnet build .\MYA.slnx
dotnet run --project .\MYA.Api\MYA.Api.csproj --launch-profile http
```

Swagger:

```text
http://127.0.0.1:5110/swagger
```
