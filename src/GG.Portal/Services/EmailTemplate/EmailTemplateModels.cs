namespace GG.Portal.Services.EmailTemplate;

public class EmailTemplateCreateCommand
{
    public required string Name { get; set; }
    public string? Subject { get; set; }
    public string? Html { get; set; }
}

public class EmailTemplateCreateResult
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string? Text { get; set; }

    public string? Html { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class EmailTemplateUpdateCommand
{
    public required string Name { get; set; }
    public string? Subject { get; set; }
    public string? Html { get; set; }
}

public class EmailTemplateGetCommand
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string? Subject { get; set; }

    public string? Html { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Updated { get; set; }
}
