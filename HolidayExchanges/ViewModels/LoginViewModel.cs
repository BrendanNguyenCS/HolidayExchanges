using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HolidayExchanges.ViewModels
{
    /// <summary>
    /// A viewmodel for the login page.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// The username field.
        /// </summary>
        [Required(ErrorMessage = "This field is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "This field must be between 3 and 50 characters long.")]
        [Display(Name = "Username")]
        [Remote("IsAValidUser", "Login", ErrorMessage = "Incorrect username")]
        public string UserName { get; set; }

        /// <summary>
        /// The password field.
        /// </summary>
        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "The password must be longer than {0} characters.")]
        [StringLength(256)]
        [Remote("IsPasswordCorrect", "Login", ErrorMessage = "Incorrect password", AdditionalFields = "UserName")]
        public string Password { get; set; }
    }
}