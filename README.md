# MYA Backend

Backend .NET 10 per login cliente, MFA via SMS/email e generazione token MYA.

## Aprire in Visual Studio

1. Clona il repository BE.
2. Apri `MYA.sln` con Visual Studio.
3. Imposta `MYA.Api` come startup project, se non e' gia' selezionato.
4. Seleziona il profilo `http`.
5. Premi `F5`.

Swagger parte su:

```text
http://127.0.0.1:5110/swagger
```

La configurazione di sviluppo e' gia' inclusa in:

```text
src/MYA.Api/appsettings.Development.json
```

## Stack

- .NET 10 / ASP.NET Core Web API
- Visual Studio Community 2022
- SQL Server con `Microsoft.Data.SqlClient` `7.0.2`
- JWT con `System.IdentityModel.Tokens.Jwt` `8.22.0`
- Swagger con `Swashbuckle.AspNetCore` `10.2.3`
- NuGet Audit attivo su dipendenze dirette e transitive

## Architettura

```text
src/
+-- MYA.Api        Controller, Swagger, CORS, DI, configurazione
+-- MYA.Business   Login, MFA, crypto legacy, token
+-- MYA.Data       DB helper async e repository SQL
+-- MYA.Models     DTO, response, options e record condivisi
database/
+-- Puzzle         Stored procedure Puzzle 10.20.0.80
+-- Sat            Stored procedure SAT 10.20.0.30
```

## Flusso Login

```text
POST /api/myAuth/login
-> valida codice cliente, login e password su Puzzle
-> usa Wus_Id_App = 2
-> password cifrata con algoritmo legacy compatibile
-> se Wus_2FA_NumCell esiste invia MFA via SMS
-> se manca cellulare ma esiste Wus_Email invia MFA via email
-> se mancano entrambi genera token e ritorna securityNotice
```

```text
POST /api/myAuth/verifyMfa
-> verifica codice MFA e scadenza
-> azzera Wus_2FA_Code dopo successo
-> genera access token JWT e refresh token
-> scrive JWTokens con idApp = 2 e permission = 0
```

## Endpoint

- `POST /api/myAuth/login`
- `POST /api/myAuth/verifyMfa`

## Configurazione Dev

Il profilo `Development` usa gia':

- Puzzle `10.20.0.80`
- SAT `10.20.0.30`
- API locale `http://127.0.0.1:5110`
- JWT signing key locale
- chiave crypto legacy
- endpoint mail `https://devapi.axitea.com/api/AuthNew/SendMail`

Per cambiare un valore, modifica `src/MYA.Api/appsettings.Development.json`.

## Comandi CLI

```powershell
dotnet restore
dotnet build .\MYA.sln
dotnet run --project .\src\MYA.Api\MYA.Api.csproj --launch-profile http
```

## Stored Procedure

Puzzle:

```text
database/Puzzle/001_sp_My_GetLogin.sql
database/Puzzle/002_sp_My_SetMfaCode.sql
database/Puzzle/003_sp_My_VerifyMfaCode.sql
database/Puzzle/004_sp_My_InsertJWTokenValue.sql
database/Puzzle/005_sp_My_GetJWTokenValue.sql
database/Puzzle/006_seed_luca_pesola_mfa_email.sql
```

SAT:

```text
database/Sat/001_sp_My_EnqueueMfaSms.sql
```

## Sicurezza Pacchetti

Controllo manuale:

```powershell
dotnet list .\MYA.sln package --vulnerable --include-transitive
```
