using System.ComponentModel.DataAnnotations;

namespace GG.Core.Models;

public class UserForgotPassword
{
    [MaxLength(256)]
    public string? Name { get; set; }

    [MaxLength(319)]
    [EmailAddress]
    public required string Email { get; set; }

    public required string Token { get; set; }
}
