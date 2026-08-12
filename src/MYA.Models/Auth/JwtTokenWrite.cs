namespace MYA.Models.Auth;

public sealed record JwtTokenWrite(
    string TokenValue,
    string Username,
    string RefreshToken,
    int AppId);
