using System.ComponentModel.DataAnnotations;

namespace GG.Auth.Dtos;

public class ApplicationRegisterDto
{
    [Required]
    public required string ClientId { get; set; }

    public string? DisplayName { get; set; }
}
