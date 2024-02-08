﻿namespace GG.Core.Dto;

public class EmailSendDto
{
    public required string Template { get; set; }
    public required EmailAddress From { get; set; }
    public required EmailAddress To { get; set; }
    public Dictionary<string, string> Variables { get; set; } = [];
    public required EmailConfiguration Configuration { get; set; }
}

public class EmailAddress
{
    public required string Email { get; set; }
    public string? Name { get; set; }
}

public class EmailConfiguration
{
    public string? UserName { get; set; }
    public string? UserPassword { get; set; }
    public required string Host { get; set; }
    public required int Port { get; set; }
    public bool UseSsl { get; set; }
}


