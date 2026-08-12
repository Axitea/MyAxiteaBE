using MYA.Models.Auth;

namespace MYA.Business.Auth;

public interface IMfaMailSender
{
    Task SendMfaCodeAsync(MfaMailMessage message, CancellationToken cancellationToken = default);
}
