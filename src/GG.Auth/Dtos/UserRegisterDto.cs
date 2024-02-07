using System.ComponentModel.DataAnnotations;

namespace GG.Auth.Dtos;

public class UserRegisterDto
{
    [Required]
    [MaxLength(319)]
    [EmailAddress]
    public required string Email { get; set; }

    [MaxLength(256)]
    public string? Name { get; set; }

    [Required]
    [MaxLength(256)]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}
