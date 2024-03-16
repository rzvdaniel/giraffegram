namespace GG.Portal.Services.ApiKey;

public class ApiKeyAdd
{
    public required string Name { get; set; }
}

public class ApiKeyCreated
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Key { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class ApiKeyGet
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Updated { get; set; }
}

public class ApiKeyUpdate
{
    public Guid Id { get; set; }

    public string? Name { get; set; }
}
