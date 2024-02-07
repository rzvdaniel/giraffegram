namespace GG.Core.Dto;

public class EmailSendDto
{
    public required EmailFrom From { get; set; }
    public required EmailTo To { get; set; }
    public string? Subject { get; set; }
    public required string Template { get; set; }
    public Dictionary<string, string> Variables { get; set; } = [];
    public required EmailServer Server { get; set; }
    public required EmailAccount Account { get; set; }
}

public class EmailFrom
{
    public required string Email { get; set; }
    public string? Name { get; set; }
}

public class EmailTo
{
    public required string Email { get; set; }
    public string? Name { get; set; }
}

public class EmailServer
{
    public required string Host { get; set; }
    public required int Port { get; set; }
    public bool UseSsl { get; set; }
}

public class EmailAccount
{
    public string? UserName { get; set; }
    public string? UserPassword { get; set; }
}


