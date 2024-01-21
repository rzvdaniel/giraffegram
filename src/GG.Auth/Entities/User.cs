using Microsoft.AspNetCore.Identity;

namespace GG.Auth.Entities;

public class User : IdentityUser<Guid>
{
    public string? Name { get; set; }
}
