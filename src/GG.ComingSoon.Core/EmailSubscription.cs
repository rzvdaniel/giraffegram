using System.ComponentModel.DataAnnotations;

namespace GG.ComingSoon.Core;

public class EmailSubscription
{
    [Key]
    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(319)]
    /// According to https://www.rfc-editor.org/rfc/rfc5321.html, 
    /// max email length should be 319 characters
    /// * 64 characters for the "local part" (username).
    /// * 1 character for the @ symbol.
    /// * 254 characters for the domain name
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public DateTime SubscribedAt { get; set; }
}
