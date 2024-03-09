namespace GG.Core.Models;

public class EmailTemplateUpdate
{
    public required string Name { get; set; }
    public string? Subject { get; set; }
    public string? Html { get; set; }
}
