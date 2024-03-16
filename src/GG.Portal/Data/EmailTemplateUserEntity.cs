#nullable disable

using System.ComponentModel.DataAnnotations;

namespace GG.Portal.Data;

public class EmailTemplateUserEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid EmailTemplateId { get; set; }
}
