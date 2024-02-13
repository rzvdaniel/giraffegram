namespace GG.Core.Config;

public class EmailConfig
{
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserPassword { get; set; } = string.Empty;
    public string ServerHost { get; set; } = string.Empty;
    public int ServerPort { get; set; } = 425;
    public bool ServerUseSsl { get; set; } = true;
}
