using Microsoft.Data.SqlClient;
using MYA.Data.Database;
using MYA.Models.Auth;

namespace MYA.Data.Repositories;

public sealed class AuthRepository : IAuthRepository
{
    private readonly IDbExecutor _db;

    public AuthRepository(IDbExecutor db)
    {
        _db = db;
    }

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

        return _db.QuerySingleOrDefaultAsync(
            DatabaseTarget.Puzzle,
            "dbo.sp_My_GetLogin",
            parameters,
            MapLoginUser,
            cancellationToken);
    }

    public async Task SetMfaCodeAsync(
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

        await _db.ExecuteAsync(DatabaseTarget.Puzzle, "dbo.sp_My_SetMfaCode", parameters, cancellationToken)
            .ConfigureAwait(false);
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

        return _db.QuerySingleOrDefaultAsync(
            DatabaseTarget.Puzzle,
            "dbo.sp_My_VerifyMfaCode",
            parameters,
            MapLoginUser,
            cancellationToken);
    }

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
}
