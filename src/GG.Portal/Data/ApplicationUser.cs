using Microsoft.AspNetCore.Identity;

namespace GG.Portal.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
    }
}
