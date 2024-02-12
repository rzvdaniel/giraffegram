using System.ComponentModel.DataAnnotations;

namespace GG.Core.Dto;

public class UserForgotPasswordDto
{
    [MaxLength(256)]
    public required string Name { get; set; }

    [MaxLength(319)]
    [EmailAddress]
    public required string Email { get; set; }

    public required string Token { get; set; }
}
