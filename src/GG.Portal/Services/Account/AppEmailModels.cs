using System.ComponentModel.DataAnnotations;

namespace GG.Portal.Services.Account;

public class UserRegistrationEmailCommand
{
    [MaxLength(319)]
    [EmailAddress]
    public required string Email { get; set; }

    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;
}


public class UserForgotPasswordCommand
{
    [MaxLength(256)]
    public string? Name { get; set; }

    [MaxLength(319)]
    [EmailAddress]
    public required string Email { get; set; }

    public required string Token { get; set; }
}


