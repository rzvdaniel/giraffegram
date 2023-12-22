using Microsoft.AspNetCore.Identity;

namespace GG.Auth.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
