using Microsoft.AspNetCore.Identity;

namespace Auth.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Gets or sets the last name for this user.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first name for this user.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date of birth for this user.
        /// </summary>
        public DateTime? DateOfBirth { get; set; } = null;

        /// <summary>
        /// Gets or sets the URI of the profile picture for this user.
        /// </summary>
        public string PictureURI { get; set; } = string.Empty;

        /// <summary>
        /// Gets the date of account creation for this user (expressed in UTC).
        /// </summary>
        public DateTime DateOfCreation { get; private set; } = DateTime.UtcNow;
    }
}
