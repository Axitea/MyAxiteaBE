using Microsoft.Data.SqlClient;
using MYA.Data.Common;
using MYA.Models.Auth;

namespace MYA.Data.Puzzle;

public sealed class PuzzleDataAccess
{
    private readonly SqlExecutor _sql;

    public PuzzleDataAccess(SqlExecutor sql)
    {
        _sql = sql;
    }

    #region Authentication

    public Task<LoginUser?> GetLoginAsync(
        string codiceCliente,
        string login,
        string encryptedPassword,
        int appId,
        CancellationToken cancellationToken = default)
    {
        SqlParameter[] parameters =
        [
            SqlParameterFactory.VarChar("@CodCliente", codiceCliente, 50),
            SqlParameterFactory.VarChar("@Login", login, 255),
            SqlParameterFactory.NVarChar("@Password", encryptedPassword, 255),
            SqlParameterFactory.Int("@IdApp", appId)
        ];

        return _sql.QuerySingleOrDefaultAsync(
            DatabaseTarget.Puzzle,
            "dbo.sp_My_GetLogin",
            parameters,
            MapLoginUser,
            cancellationToken);
    }

    public Task SaveMfaCodeAsync(
        long userId,
        int appId,
        string code,
        DateTimeOffset requestedAt,
        CancellationToken cancellationToken = default)
    {
        SqlParameter[] parameters =
        [
            SqlParameterFactory.BigInt("@IdUtente", userId),
            SqlParameterFactory.Int("@IdApp", appId),
            SqlParameterFactory.NVarChar("@Code", code, 6),
            SqlParameterFactory.DateTime("@RequestedAtUtc", requestedAt)
        ];

        return _sql.ExecuteAsync(DatabaseTarget.Puzzle, "dbo.sp_My_SetMfaCode", parameters, cancellationToken);
    }

    public Task<LoginUser?> VerifyMfaCodeAsync(
        long userId,
        int appId,
        string code,
        DateTimeOffset now,
        int ttlMinutes,
        CancellationToken cancellationToken = default)
    {
        SqlParameter[] parameters =
        [
            SqlParameterFactory.BigInt("@IdUtente", userId),
            SqlParameterFactory.Int("@IdApp", appId),
            SqlParameterFactory.NVarChar("@Code", code, 6),
            SqlParameterFactory.DateTime("@NowUtc", now),
            SqlParameterFactory.Int("@TtlMinutes", ttlMinutes)
        ];

        return _sql.QuerySingleOrDefaultAsync(
            DatabaseTarget.Puzzle,
            "dbo.sp_My_VerifyMfaCode",
            parameters,
            MapLoginUser,
            cancellationToken);
    }

    #endregion

    #region Tokens

    public Task SaveIssuedTokenAsync(JwtTokenWrite token, CancellationToken cancellationToken = default)
    {
        SqlParameter[] parameters =
        [
            SqlParameterFactory.NVarCharMax("@TokenValue", token.TokenValue),
            SqlParameterFactory.NVarChar("@Username", token.Username, 30),
            SqlParameterFactory.Int("@Permission", 0),
            SqlParameterFactory.NVarChar("@RefreshToken", token.RefreshToken, 200),
            SqlParameterFactory.Int("@IdApp", token.AppId)
        ];

        return _sql.ExecuteAsync(DatabaseTarget.Puzzle, "dbo.sp_My_InsertJWTokenValue", parameters, cancellationToken);
    }

    public Task<JwtTokenRecord?> GetTokenByRefreshTokenAsync(
        string refreshToken,
        int appId,
        CancellationToken cancellationToken = default)
    {
        SqlParameter[] parameters =
        [
            SqlParameterFactory.NVarChar("@RefreshToken", refreshToken, 200),
            SqlParameterFactory.Int("@IdApp", appId)
        ];

        return _sql.QuerySingleOrDefaultAsync(
            DatabaseTarget.Puzzle,
            "dbo.sp_My_GetJWTokenValue",
            parameters,
            MapToken,
            cancellationToken);
    }

    #endregion

    private static LoginUser MapLoginUser(SqlDataReader reader)
    {
        return new LoginUser(
            UserIdToken: reader.GetInt64Required("Wus_IdUtenteToken"),
            UserId: reader.GetInt64Required("Wus_IdUtente"),
            Login: reader.GetNullableString("Wus_Login") ?? string.Empty,
            DisplayName: reader.GetNullableString("Wus_Nome") ?? string.Empty,
            Permission: reader.GetNullableInt32("Wus_Permessi") ?? 0,
            Email: reader.GetNullableString("Wus_Email"),
            CustomerName: reader.GetNullableString("Wus_CliRagSociale"),
            ScenarioId: reader.GetNullableInt32("Wus_IdScenario"),
            Language: reader.GetNullableString("Wus_Lingua"),
            PhoneNumber: reader.GetNullableString("Wus_2FA_NumCell"),
            IsMfaActive: string.Equals(reader.GetNullableString("Wus_2FA_IsActive")?.Trim(), "S", StringComparison.OrdinalIgnoreCase),
            IsMfaLocked: reader.GetNullableBoolean("Wus_2FA_Locked") ?? false);
    }

    private static JwtTokenRecord MapToken(SqlDataReader reader)
    {
        return new JwtTokenRecord(
            IdToken: reader.GetInt32Required("idToken"),
            TokenValue: reader.GetNullableString("tokenValue") ?? string.Empty,
            Username: reader.GetNullableString("username") ?? string.Empty,
            DateToken: reader.GetDateTimeRequired("dateToken"),
            Permission: reader.GetNullableInt32("permission") ?? 0,
            RefreshToken: reader.GetNullableString("refreshToken") ?? string.Empty,
            IdApp: reader.GetNullableInt32("idApp"));
    }
}
