#nullable disable

using System.ComponentModel.DataAnnotations;

namespace GG.Core.Entities;

public class EmailTemplateUser
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid EmailTemplateId { get; set; }
}
