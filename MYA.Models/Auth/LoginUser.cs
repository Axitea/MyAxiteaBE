namespace MYA.Models.Auth;

public sealed record LoginUser(
    long UserIdToken,
    long UserId,
    string Login,
    string DisplayName,
    int Permission,
    string? Email,
    string? CustomerName,
    int? ScenarioId,
    string? Language,
    string? PhoneNumber,
    bool IsMfaActive,
    bool IsMfaLocked);
