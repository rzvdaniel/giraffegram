namespace GG.Portal.Services.EmailTemplate;

public class EmailTemplateAdd
{
    public required string Name { get; set; }
    public string? Subject { get; set; }
    public string? Html { get; set; }
}

public class EmailTemplateUpdate
{
    public required string Name { get; set; }
    public string? Subject { get; set; }
    public string? Html { get; set; }
}

public class EmailTemplateGet
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string? Subject { get; set; }

    public string? Html { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Updated { get; set; }
}

public class EmailTemplateCreated
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string? Text { get; set; }

    public string? Html { get; set; }

    public DateTime CreatedAt { get; set; }
}
