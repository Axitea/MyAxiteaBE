using MYA.Models.Auth;

namespace MYA.Data.Repositories;

public interface ITokenRepository
{
    Task InsertAsync(JwtTokenWrite token, CancellationToken cancellationToken = default);

    Task<JwtTokenRecord?> GetByRefreshTokenAsync(
        string refreshToken,
        int appId,
        CancellationToken cancellationToken = default);
}
