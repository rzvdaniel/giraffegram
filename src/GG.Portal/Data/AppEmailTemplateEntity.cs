#nullable disable

using System.ComponentModel.DataAnnotations;

namespace GG.Portal.Data;

public class AppEmailTemplateEntity
{
    [Key]
    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(256)]
    public required string Name { get; set; }

    public string Subject { get; set; }

    public string Html { get; set; }

    [Required]
    public DateTime Created { get; set; }

    public DateTime? Updated { get; set; }
}
