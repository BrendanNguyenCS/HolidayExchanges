using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HolidayExchanges.ViewModels
{
    /// <summary>
    /// The viewmodel for the password reset page.
    /// </summary>
    public class ForgotPasswordVM
    {
        /// <summary>
        /// The username of the desired account.
        /// </summary>
        [Required]
        [Display(Name = "Username")]
        [Remote("IsAValidUser", "Login", ErrorMessage = "Incorrect username")]
        public string UserName { get; set; }

        /// <summary>
        /// The user's new password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "The password must be longer than {0} characters.")]
        [StringLength(256)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        /// <summary>
        /// New password verification.
        /// </summary>
        [Required(ErrorMessage = "You must verify your password.")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "This does not match your new password.")]
        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}