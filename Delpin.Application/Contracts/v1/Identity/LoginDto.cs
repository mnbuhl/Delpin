using System.ComponentModel.DataAnnotations;

namespace Delpin.Application.Contracts.v1.Identity
{
    public class LoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}