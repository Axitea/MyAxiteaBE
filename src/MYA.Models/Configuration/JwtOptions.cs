namespace MYA.Models.Configuration;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "MYA.Api";

    public string Audience { get; set; } = "MYA.FE";

    public string SigningKey { get; set; } = string.Empty;

    public int AccessTokenMinutes { get; set; } = 60;

    public int RefreshTokenDays { get; set; } = 30;
}
