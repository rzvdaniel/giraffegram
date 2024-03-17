#nullable disable

using System.ComponentModel.DataAnnotations;

namespace GG.Portal.Data;

public class ApiKeyUser
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid ApiKeyId { get; set; }
}
