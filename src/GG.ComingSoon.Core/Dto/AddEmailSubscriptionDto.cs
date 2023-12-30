using System.ComponentModel.DataAnnotations;

namespace GG.ComingSoon.Core.Dto;

public class AddEmailSubscriptionDto
{
    [Required]
    [MaxLength(319)]
    [EmailAddress]
    // TODO! Email is required
    public string? Email { get; set; }
}
