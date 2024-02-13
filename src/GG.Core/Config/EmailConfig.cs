namespace GG.Core.Config;

public class EmailConfig
{
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? UserPassword { get; set; }
    public string? ServerHost { get; set; }
    public int ServerPort { get; set; } = 425;
    public bool ServerUseSsl { get; set; } = true;
}
