namespace GG.Core.Dto;

public class EmailTemplateAddDto
{
    public required string Name { get; set; }
    public string? Text { get; set; }
    public string? Html { get; set; }
}
