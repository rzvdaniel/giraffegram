namespace GG.Core.Dto;

public class EmailAccountGetDto
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Host { get; set; }

    public required int Port { get; set; }

    public bool? UseSsl { get; set; }

    public required string UserName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
