namespace GG.Core.Dto;

public class ApiKeyGetDto
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Updated { get; set; }
}
