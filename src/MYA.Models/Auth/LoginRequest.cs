using System.ComponentModel.DataAnnotations;

namespace MYA.Models.Auth;

public sealed class LoginRequest
{
    [Required]
    [MaxLength(50)]
    public string CodiceCliente { get; init; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Login { get; init; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Password { get; init; } = string.Empty;
}
