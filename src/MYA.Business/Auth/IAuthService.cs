using MYA.Models.Auth;

namespace MYA.Business.Auth;

public interface IAuthService
{
    Task<LoginStartedResponse> StartLoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<TokenResponse> VerifyMfaAsync(VerifyMfaRequest request, CancellationToken cancellationToken = default);
}
