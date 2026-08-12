using MYA.Models.Auth;

namespace MYA.Data.Repositories;

public interface IAuthRepository
{
    Task<LoginUser?> GetLoginAsync(
        string codiceCliente,
        string login,
        string encryptedPassword,
        int appId,
        CancellationToken cancellationToken = default);

    Task SetMfaCodeAsync(
        long userId,
        int appId,
        string code,
        DateTimeOffset requestedAt,
        CancellationToken cancellationToken = default);

    Task<LoginUser?> VerifyMfaCodeAsync(
        long userId,
        int appId,
        string code,
        DateTimeOffset now,
        int ttlMinutes,
        CancellationToken cancellationToken = default);
}
