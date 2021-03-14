using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HolidayExchanges.Models
{
    /// <summary>
    /// Represents the many-to-many relationship between <see cref="HolidayExchanges.Models.User"/>
    /// and <see cref="HolidayExchanges.Models.Group"/>
    /// </summary>
    /// <remarks>
    /// The <see cref="HolidayExchanges.Models.UserGroup.UserID"/> and <see
    /// cref="HolidayExchanges.Models.UserGroup.GroupID"/> serve as a composite primary key, meaning
    /// that the combination has be unique (not repeating 1,1 and so on).
    /// </remarks>
    [Table("UserGroup")]
    public class UserGroup
    {
        /// <summary>
        /// The identifier of the group participant ( <see cref="HolidayExchanges.Models.UserGroup.User"/>)
        /// </summary>
        /// <remarks>Left column of composite primary key, also a foreign key to <see cref="HolidayExchanges.Models.User"/></remarks>
        [Key, Column(Order = 0)]
        [ForeignKey("User")]
        public int UserID { get; set; }

        /// <summary>
        /// The identifier of the group that the user is in
        /// </summary>
        /// <remarks>Right column of composite primary key</remarks>
        [Key, Column(Order = 1)]
        [ForeignKey("Group")]
        public int GroupID { get; set; }

        /// <summary>
        /// The identifier of the <see cref="HolidayExchanges.Models.UserGroup.User"/>'s match (
        /// <see cref="HolidayExchanges.Models.UserGroup.Recipient"/>)
        /// </summary>
        /// <remarks>Not a foreign key</remarks>
        public int RecipientUserID { get; set; }

        /// <summary>
        /// Navigation property for <see cref="HolidayExchanges.Models.UserGroup.UserID"/>
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Navigation property for <see cref="HolidayExchanges.Models.UserGroup.GroupID"/>
        /// </summary>
        public virtual Group Group { get; set; }
    }
}