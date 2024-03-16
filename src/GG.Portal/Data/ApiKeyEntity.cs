#nullable disable

using System.ComponentModel.DataAnnotations;

namespace GG.Portal.Data;

public class ApiKeyEntity
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

    public virtual ICollection<ApiKeyUserEntity> ApiKeyUsers { get; set; }
}
