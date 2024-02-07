using System.ComponentModel.DataAnnotations;

namespace GG.Auth.Dtos;

public class UserRegisterDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.")]
    public string? Name { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}
