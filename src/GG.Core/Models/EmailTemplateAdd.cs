namespace GG.Core.Models;

public class EmailTemplateAdd
{
    public required string Name { get; set; }
    public string? Subject { get; set; }
    public string? Html { get; set; }
}
