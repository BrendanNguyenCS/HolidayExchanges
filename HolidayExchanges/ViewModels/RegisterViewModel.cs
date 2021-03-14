using HolidayExchanges.Models;
using System.ComponentModel.DataAnnotations;

namespace HolidayExchanges.ViewModels
{
    public class RegisterViewModel
    {
        public User User { get; set; }

        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        public bool ValidatePassword()
        {
            return ConfirmPassword.Equals(User.Password);
        }
    }
}