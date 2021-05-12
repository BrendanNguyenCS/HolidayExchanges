using System.ComponentModel.DataAnnotations;

namespace HolidayExchanges.ViewModels
{
    public class ContactViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Not a valid name. Must be between 5 and 50 characters long.")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Not a valid email address.")]
        public string Email { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [StringLength(1000)]
        public string Message { get; set; }
    }
}