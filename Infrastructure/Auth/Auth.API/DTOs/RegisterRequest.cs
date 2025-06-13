using System.ComponentModel.DataAnnotations;

namespace Auth.API.DTOs
{
    public class RegisterRequest
    {
        /// <summary>
        /// Optional username. Used for login if not using email.
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Email. Can be used for login.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Required password.
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Required confirmation password.
        /// </summary>
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Optional first name.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Optional last name.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Optional date of birth.
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Optional phone number.
        /// </summary>
        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
