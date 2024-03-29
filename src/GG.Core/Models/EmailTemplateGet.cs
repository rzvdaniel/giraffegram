﻿namespace GG.Core.Models;

public class EmailTemplateGet
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string? Subject { get; set; }

    public string? Html { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Updated { get; set; }
}
