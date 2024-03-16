namespace GG.Portal.Services.ApiKey;

public class ApiKeyCreateCommand
{
    public required string Name { get; set; }
}

public class ApiKeyCreateResult
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Key { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class ApiKeyGetQuery
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Updated { get; set; }
}

public class ApiKeyUpdateCommand
{
    public Guid Id { get; set; }

    public string? Name { get; set; }
}
