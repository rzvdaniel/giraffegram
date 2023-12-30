namespace GG.Core.Models
{
    public class Email
    {
        public string? FromName { get; set; }
        public required string FromAddress { get; set; }
        public string? ToName { get; set; }
        public required string ToAddress { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }
        public string? Server { get; set; }
    }
}
