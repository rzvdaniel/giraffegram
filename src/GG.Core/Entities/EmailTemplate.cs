#nullable disable

using System.ComponentModel.DataAnnotations;

namespace GG.Core.Entities;

public class EmailTemplate
{
    [Key]
    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(256)]
    public required string Name { get; set; }

    [Required]
    public required string Body { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<EmailTemplateUser> EmailTemplateUsers { get; set; }
}
