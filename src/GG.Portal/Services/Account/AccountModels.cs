using System.ComponentModel.DataAnnotations;

namespace GG.Portal.Services.Account;

public class UserRegistrationCommand
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

public class UserProfileUpdateCommand
{
    public Guid Id { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string UserName { get; set; }

    public required string Email { get; set; }
}


