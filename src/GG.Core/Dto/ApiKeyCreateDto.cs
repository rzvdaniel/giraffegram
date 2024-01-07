namespace GG.Core.Dto;

public class ApiKeyCreateDto
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Key { get; set; }

    public DateTime CreatedAt { get; set; }
}
