namespace MYA.Models.Auth;

public sealed record MfaSmsMessage(
    long UserId,
    string PhoneNumber,
    string Text,
    DateTimeOffset RequestedAt);
