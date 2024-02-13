using System.ComponentModel.DataAnnotations;

namespace GG.Core.Dto;

public class UserRegisterDto
{
    [MaxLength(319)]
    [EmailAddress]
    public required string Email { get; set; }

    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(256)]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}
