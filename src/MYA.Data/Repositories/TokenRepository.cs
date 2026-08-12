using Microsoft.Data.SqlClient;
using MYA.Data.Database;
using MYA.Models.Auth;

namespace MYA.Data.Repositories;

public sealed class TokenRepository : ITokenRepository
{
    private readonly IDbExecutor _db;

    public TokenRepository(IDbExecutor db)
    {
        _db = db;
    }

    public async Task InsertAsync(JwtTokenWrite token, CancellationToken cancellationToken = default)
    {
        SqlParameter[] parameters =
        [
            SqlParameterFactory.NVarCharMax("@TokenValue", token.TokenValue),
            SqlParameterFactory.NVarChar("@Username", token.Username, 30),
            SqlParameterFactory.Int("@Permission", 0),
            SqlParameterFactory.NVarChar("@RefreshToken", token.RefreshToken, 200),
            SqlParameterFactory.Int("@IdApp", token.AppId)
        ];

        await _db.ExecuteAsync(DatabaseTarget.Puzzle, "dbo.sp_My_InsertJWTokenValue", parameters, cancellationToken)
            .ConfigureAwait(false);
    }

    public Task<JwtTokenRecord?> GetByRefreshTokenAsync(
        string refreshToken,
        int appId,
        CancellationToken cancellationToken = default)
    {
        SqlParameter[] parameters =
        [
            SqlParameterFactory.NVarChar("@RefreshToken", refreshToken, 200),
            SqlParameterFactory.Int("@IdApp", appId)
        ];

        return _db.QuerySingleOrDefaultAsync(
            DatabaseTarget.Puzzle,
            "dbo.sp_My_GetJWTokenValue",
            parameters,
            MapToken,
            cancellationToken);
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
