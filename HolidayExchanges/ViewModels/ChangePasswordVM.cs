//using HolidayExchanges.Components;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HolidayExchanges.ViewModels
{
    /// <summary>
    /// A viewmodel for the change password page.
    /// </summary>
    public class ChangePasswordVM
    {
        public string Username { get; set; }

        /// <summary>
        /// The user's current password.
        /// </summary>
        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        [Remote("IsPasswordCorrect", "Login", ErrorMessage = "Incorrect password", AdditionalFields = "Username")]
        public string CurrentPassword { get; set; }

        /// <summary>
        /// The user's desired new password.
        /// </summary>
        [Required(ErrorMessage = "You must enter a new password")]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [StringLength(256)]
        [MinLength(5, ErrorMessage = "The password must be longer than {0} characters.")]
        //[Unlike("CurrentPassword")]
        public string NewPassword { get; set; }

        /// <summary>
        /// The password change confirmation field.
        /// </summary>
        [Required(ErrorMessage = "You must confirm your new password.")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "This field must match the new password field.")]
        public string ConfirmPassword { get; set; }
    }
}