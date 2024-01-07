#nullable disable

using System.ComponentModel.DataAnnotations;

namespace GG.Core.Entities;

public class ApiKey
{
    [Key]
    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(256)]
    public required string Name { get; set; }

    [Required]
    [MaxLength(256)]
    public required string Key { get; set; }

    [Required]
    public DateTime Created { get; set; }

    public DateTime? Updated { get; set; }

    public virtual ICollection<ApiKeyUser> ApiKeyUsers { get; set; }
}
