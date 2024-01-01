namespace GG.Core.Dto;

public class SendEmailDto
{
    public string? FromName { get; set; }
    public required string FromAddress { get; set; }
    public string? ToName { get; set; }
    public required string ToAddress { get; set; }
    public string? Subject { get; set; }
    public string? Message { get; set; }
    public required string Server { get; set; }
}
