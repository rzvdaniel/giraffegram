using System.ComponentModel.DataAnnotations;

namespace GG.Auth.Models
{
    public class ResetPassword
    {
        [MaxLength(256)]
        [EmailAddress]
        public required string Email { get; set; }

        public required string Token { get; set; }

        [MaxLength(256)]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
