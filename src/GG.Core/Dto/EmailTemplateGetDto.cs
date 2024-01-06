namespace GG.Core.Dto;

public class EmailTemplateGetDto
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string? Text { get; set; }

    public string? Html { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
