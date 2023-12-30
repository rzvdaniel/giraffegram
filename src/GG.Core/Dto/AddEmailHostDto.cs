namespace GG.Core.Dto
{
    public class AddEmailHostDto
    {
        public required string Name { get; set; }
        public required string Host { get; set; }
        public required string Port { get; set; }
        public bool UseSsl { get; set; }
        public required string UserName { get; set; }
        public required string UserPassword { get; set; }
    }
}
