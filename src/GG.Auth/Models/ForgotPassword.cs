using System.ComponentModel.DataAnnotations;

namespace GG.Auth.Models
{
    public class ForgotPassword
    {
        [EmailAddress]
        public required string Email { get; set; }
    }
}
