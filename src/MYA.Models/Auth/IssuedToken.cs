namespace MYA.Models.Auth;

public sealed record IssuedToken(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiresAt);
