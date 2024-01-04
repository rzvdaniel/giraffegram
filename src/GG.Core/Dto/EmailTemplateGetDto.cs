﻿namespace GG.Core.Dto;

public class EmailTemplateGetDto
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Body { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
