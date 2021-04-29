using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace HolidayExchanges.ViewModels
{
    /// <summary>
    /// The viewmodel for the user profile editing view.
    /// </summary>
    public class UserEditViewModel
    {
        /// <summary>
        /// The user identifier.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The username for the current user.
        /// </summary>
        [Required(ErrorMessage = "This field is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "This field must be between 3 and 50 characters long.")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

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
        [EmailAddress(ErrorMessage = "Email is invalid")]
        [Column(Order = 4)]
        [Remote("IsEmailAvailableOnEdit", "Login", ErrorMessage = "This email is associated with another user already.", AdditionalFields = "OriginalEmail")]
        public string Email { get; set; }

        /// <summary>
        /// A property used for email availability validation.
        /// </summary>
        [StringLength(128)]
        [DataType(DataType.EmailAddress)]
        public string OriginalEmail { get; set; }

        /// <summary>
        /// The user's birthday.
        /// </summary>
        [Column(TypeName = "date")]
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