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

    public string? Text { get; set; }

    public string? Html { get; set; }

    [Required]
    public DateTime Created { get; set; }

    public DateTime? Updated { get; set; }

    public virtual ICollection<EmailTemplateUser> EmailTemplateUsers { get; set; }
}
