namespace MYA.Models.Auth;

public sealed record LoginStartedResponse(
    long UserId,
    string Login,
    string DisplayName,
    string? MaskedPhoneNumber,
    string MfaDeliveryChannel,
    string? MaskedDestination,
    DateTimeOffset? MfaExpiresAt,
    bool RequiresMfa,
    TokenResponse? Token,
    string? SecurityNotice);
