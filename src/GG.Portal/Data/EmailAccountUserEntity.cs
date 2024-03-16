#nullable disable

using System.ComponentModel.DataAnnotations;

namespace GG.Portal.Data;

public class EmailAccountUserEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid EmailAccountId { get; set; }
}
