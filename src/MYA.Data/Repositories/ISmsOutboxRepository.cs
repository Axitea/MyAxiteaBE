using MYA.Models.Auth;

namespace MYA.Data.Repositories;

public interface ISmsOutboxRepository
{
    Task EnqueueMfaSmsAsync(MfaSmsMessage message, CancellationToken cancellationToken = default);
}
