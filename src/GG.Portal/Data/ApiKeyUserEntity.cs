#nullable disable

using System.ComponentModel.DataAnnotations;

namespace GG.Portal.Data;

public class ApiKeyUserEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid ApiKeyId { get; set; }
}
