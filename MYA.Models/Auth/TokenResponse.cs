namespace MYA.Models.Auth;

public sealed record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiresAt,
    long UserId,
    string Login,
    int AppId);
