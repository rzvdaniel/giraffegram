using System.ComponentModel.DataAnnotations;

namespace GG.Auth.Entities;

public class ClientUser
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string ClientId { get; set; }

}
