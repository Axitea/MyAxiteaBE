namespace MYA.Models.Auth;

public sealed record JwtTokenRecord(
    int IdToken,
    string TokenValue,
    string Username,
    DateTime DateToken,
    int Permission,
    string RefreshToken,
    int? IdApp);
