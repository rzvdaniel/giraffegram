namespace GG.Core.Models;

public class EmailTemplateCreated
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string? Text { get; set; }

    public string? Html { get; set; }

    public DateTime CreatedAt { get; set; }
}
