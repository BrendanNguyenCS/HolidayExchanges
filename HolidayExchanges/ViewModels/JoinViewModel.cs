using System;
using System.ComponentModel.DataAnnotations;

namespace HolidayExchanges.ViewModels
{
    /// <summary>
    /// A viewmodel for the group join page.
    /// </summary>
    public class JoinViewModel
    {
        /// <summary>
        /// The group identifier.
        /// </summary>
        public int GroupID { get; set; }

        /// <summary>
        /// The group name.
        /// </summary>
        [Display(Name = "Name")]
        public string GroupName { get; set; }

        /// <summary>
        /// The username of the group creator.
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// The group's exchange date.
        /// </summary>
        [Display(Name = "Exchange Date")]
        [DataType(DataType.Date)]
        public DateTime ExchangeDate { get; set; }
    }
}