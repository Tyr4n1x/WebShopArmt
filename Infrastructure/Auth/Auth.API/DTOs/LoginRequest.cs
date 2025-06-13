using System.ComponentModel.DataAnnotations;

namespace Auth.API.DTOs
{
    public class LoginRequest
    {
        /// <summary>
        /// The user's login entry. Can be either username or email.
        /// </summary>
        [Required]
        public string Entry { get; set; } = string.Empty;

        /// <summary>
        /// The user's password.
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
