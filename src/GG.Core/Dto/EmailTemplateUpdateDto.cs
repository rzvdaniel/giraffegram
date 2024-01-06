namespace GG.Core.Dto;

public class EmailTemplateUpdateDto
{
    public required string Name { get; set; }
    public string? Text { get; set; }
    public string? Html { get; set; }
}
