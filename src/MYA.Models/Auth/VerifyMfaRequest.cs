using System.ComponentModel.DataAnnotations;

namespace MYA.Models.Auth;

public sealed class VerifyMfaRequest
{
    [Range(1, long.MaxValue)]
    public long UserId { get; init; }

    [Required]
    [RegularExpression("^[0-9]{6}$")]
    public string Code { get; init; } = string.Empty;
}
