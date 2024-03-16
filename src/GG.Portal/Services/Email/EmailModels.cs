namespace GG.Portal.Services.Email;

public class SendEmailCommand
{
    public required string Template { get; set; }
    public required EmailAccount From { get; set; }
    public required EmailAccount To { get; set; }
    public Dictionary<string, string> Variables { get; set; } = [];
    public required EmailConfiguration Configuration { get; set; }
}

public class EmailAccount
{
    public required string Email { get; set; }
    public string? Name { get; set; }
}

public class EmailConfiguration
{
    public required string UserName { get; set; }
    public required string UserPassword { get; set; }
    public required string Host { get; set; }
    public int Port { get; set; }
    public bool UseSsl { get; set; }
}

public class EmailResult
{
    public string? Subject { get; set; }
    public string? Html { get; set; }
}


