using System.ComponentModel.DataAnnotations;

namespace GG.Auth.Entities;

public class ApplicationUser
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public required string ClientId { get; set; }

}
