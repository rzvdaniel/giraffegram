namespace GG.Core.Models;

public class ApiKeyCreated
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Key { get; set; }

    public DateTime CreatedAt { get; set; }
}
