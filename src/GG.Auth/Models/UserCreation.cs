using System.ComponentModel.DataAnnotations;

namespace GG.Auth.Models;

public class UserCreation
{
    [MaxLength(319)]
    [EmailAddress]
    public required string Email { get; set; }

    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;
}
