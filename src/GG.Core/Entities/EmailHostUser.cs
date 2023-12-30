#nullable disable

using System.ComponentModel.DataAnnotations;

namespace GG.Core.Entities;

public class EmailHostUser
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid EmailHostId { get; set; }
}
