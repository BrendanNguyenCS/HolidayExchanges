using System.ComponentModel.DataAnnotations;

namespace HolidayExchanges.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "This field is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "This field must be between 3 and 50 characters long.")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "The password must be longer than {0} characters.")]
        [StringLength(256)]
        public string Password { get; set; }
    }
}