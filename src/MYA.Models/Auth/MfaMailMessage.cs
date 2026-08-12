namespace MYA.Models.Auth;

public sealed record MfaMailMessage(
    long UserId,
    string To,
    string Subject,
    string Body,
    string SenderDescription,
    DateTimeOffset RequestedAt);
