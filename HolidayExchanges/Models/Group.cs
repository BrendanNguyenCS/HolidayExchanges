using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HolidayExchanges.Models
{
    /// <summary>
    /// Represents a group of multiple <seealso cref="HolidayExchanges.Models.User"/> objects
    /// participating in a holiday gift exchange
    /// </summary>
    [Table("Group")]
    public class Group
    {
        /// <summary>
        /// A group's unique identifier
        /// </summary>
        public int GroupID { get; set; }

        /// <summary>
        /// The name of the group
        /// </summary>
        [StringLength(128)]
        [Display(Name = "Group Name")]
        public string Name { get; set; }

        /// <summary>
        /// The date of the gift exchange
        /// </summary>
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [Display(Name = "Exchange Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ExchangeDate { get; set; }

        [Display(Name = "Already Paired?")]
        public bool HasBeenPaired { get; set; }

        // represents username of user that created the group
        //public string Creator { get; set; }

        /// <summary>
        /// The list of <see cref="HolidayExchanges.Models.UserGroup"/> that are currently in the group.
        /// </summary>
        public virtual ICollection<UserGroup> UserGroups { get; set; }
    }
}