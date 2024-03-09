using System.ComponentModel.DataAnnotations;

namespace GG.Core.Dto;

public class UserDetailsDto
{
    [MaxLength(319)]
    [EmailAddress]
    public required string Email { get; set; }

    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;
}
