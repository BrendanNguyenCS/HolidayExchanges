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
        [Column(TypeName = "datetime")]
        [DataType(DataType.Date)]
        [Display(Name = "Exchange Date")]
        public DateTime? ExchangeDate { get; set; }

        //public bool HasBeenPaired { get; set; }

        /// <summary>
        /// The list of <see cref="HolidayExchanges.Models.UserGroup"/> that are currently in the group.
        /// </summary>
        public virtual ICollection<UserGroup> UserGroups { get; set; }
    }
}