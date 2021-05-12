using System;
using System.ComponentModel.DataAnnotations;

namespace HolidayExchanges.ViewModels
{
    /// <summary>
    /// The viewmodel for the group creation page.
    /// </summary>
    public class GroupCreateVM
    {
        [Required(ErrorMessage = "You must enter a group name.")]
        [StringLength(128)]
        [Display(Name = "Group Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "You must enter an exchange date.")]
        [DataType(DataType.Date)]
        [Display(Name = "Exchange Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ExchangeDate { get; set; }
    }
}