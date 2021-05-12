using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HolidayExchanges.ViewModels
{
    /// <summary>
    /// The viewmodel for the registration page.
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// The username for the current user.
        /// </summary>
        [Required(ErrorMessage = "This field is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "This field must be between 3 and 50 characters long.")]
        [Display(Name = "Username")]
        [Remote("IsUsernameAvailable", "Login", ErrorMessage = "This username is already taken.")]
        public string UserName { get; set; }

        /// <summary>
        /// The user's password (if from db, will be in hashed form).
        /// </summary>
        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "The password must be longer than {0} characters.")]
        [StringLength(256)]
        public string Password { get; set; }

        /// <summary>
        /// The password confirmation field.
        /// </summary>
        [Required(ErrorMessage = "You must verify your password")]
        [Display(Name = "Confirm Password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Your password confirmation does not match the previous field.")]
        [DataType(DataType.Password)]
        [StringLength(256)]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// The user's first name.
        /// </summary>
        [Display(Name = "First Name")]
        [StringLength(20)]
        public string FirstName { get; set; }

        /// <summary>
        /// The user's last name.
        /// </summary>
        [Display(Name = "Last Name")]
        [StringLength(20)]
        public string LastName { get; set; }

        /// <summary>
        /// The first line of the user's address.
        /// </summary>
        [StringLength(128)]
        [Display(Name = "Address Line 1")]
        public string Address1 { get; set; }

        /// <summary>
        /// The second line of the user's address (mainly for apartment number, suite number, P.O
        /// box, etc.).
        /// </summary>
        [StringLength(128)]
        [Display(Name = "Address Line 2")]
        public string Address2 { get; set; }

        /// <summary>
        /// The city/town of the address.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "City/Town")]
        public string City { get; set; }

        /// <summary>
        /// The state/region of the address.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "State/Province/Region")]
        public string State { get; set; }

        /// <summary>
        /// The ZIP or postal code of the address.
        /// </summary>
        [StringLength(10)]
        [Display(Name = "ZIP Code")]
        [DataType(DataType.PostalCode, ErrorMessage = "Not a valid postal code.")]
        public string Zip { get; set; }

        /// <summary>
        /// The country of the address.
        /// </summary>
        [StringLength(50)]
        public string Country { get; set; }

        /// <summary>
        /// The email address associated with the user.
        /// </summary>
        [StringLength(128)]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Not a valid email address.")]
        [Remote("IsEmailAvailable", "Login", ErrorMessage = "This email is associated with another user already.")]
        public string Email { get; set; }

        /// <summary>
        /// The user's birthday.
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// The phone number associated with the user.
        /// </summary>
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
    }
}