using System.ComponentModel.DataAnnotations;

namespace GG.ComingSoon.Core.Dto;

public class AddEmailSubscriptionDto
{
    [Required]
    [MaxLength(319)]
    [EmailAddress]
    public string? Email { get; set; }
}
