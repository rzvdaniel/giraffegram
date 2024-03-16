using System.ComponentModel.DataAnnotations;

namespace GG.Portal.Services.AppEmail;

public class AppUserDetails
{
    [MaxLength(319)]
    [EmailAddress]
    public required string Email { get; set; }

    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;
}


public class AppUserForgotPassword
{
    [MaxLength(256)]
    public string? Name { get; set; }

    [MaxLength(319)]
    [EmailAddress]
    public required string Email { get; set; }

    public required string Token { get; set; }
}


