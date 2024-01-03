namespace GG.Auth.Models;

public class ClientRegister
{
    public required string ClientId { get; set; }

    public required string ClientPassword { get; set; }

    public string? DisplayName { get; set; }
}
