namespace GG.Portal.Services.AppConfig;

public class AppConfig
{
    public string? DatabaseType { get; set; }
    public string? WebsiteUrl { get; set; }
    public bool AllowUserRegistration { get; set; }
}

public class EmailConfig
{
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? UserPassword { get; set; }
    public string? ServerHost { get; set; }
    public int ServerPort { get; set; } = 425;
    public bool ServerUseSsl { get; set; } = true;
}

public class UserPasswordConfig
{
    public bool RequireDigit { get; set; }
    public bool RequireLowercase { get; set; }
    public bool RequireUppercase { get; set; }
    public bool RequireNonAlphanumeric { get; set; }
    public int RequiredLength { get; set; }
}
