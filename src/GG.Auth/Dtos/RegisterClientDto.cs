using System.ComponentModel.DataAnnotations;

namespace GG.Auth.Dtos;

public class RegisterClientDto
{
    [Required]
    [Display(Name = "Client Id")]
    public required string ClientId { get; set; }

    public string? DisplayName { get; set; }
}
