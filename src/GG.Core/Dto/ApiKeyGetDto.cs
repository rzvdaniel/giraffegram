namespace GG.Core.Dto;

public class ApiKeyGetDto
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
