namespace GG.Core.Dto;

public class EmailSendDto
{
    public string? FromName { get; set; }
    public required string FromAddress { get; set; }
    public string? ToName { get; set; }
    public required string ToAddress { get; set; }
    public string? Subject { get; set; }
    public string? Html { get; set; }
    public string? Text { get; set; }

    public required string Host { get; set; }
    public required int Port { get; set; }
    public bool UseSsl { get; set; }
    public string? UserName { get; set; }
    public string? UserPassword { get; set; }
}
