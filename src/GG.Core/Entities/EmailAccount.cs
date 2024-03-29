﻿#nullable disable

using System.ComponentModel.DataAnnotations;

namespace GG.Core.Entities;

public class EmailAccount
{
    [Key]
    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(256)]
    public required string Name { get; set; }

    [Required]
    [MaxLength(256)]
    public required string Host { get; set; }

    [Required]
    public required int Port { get; set; }

    public bool? UseSsl { get; set; }

    [Required]
    public DateTime Created { get; set; }

    public DateTime? Updated { get; set; }

    public virtual ICollection<EmailAccountUser> EmailAccountUsers { get; set; }
}
