using HolidayExchanges.Models;
using System.Collections.Generic;

namespace HolidayExchanges.ViewModels
{
    /// <summary>
    /// The viewmodel for the user's details view.
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        /// The current <see cref="User"/>
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// The list of groups that the user is participating in.
        /// </summary>
        public List<Group> Groups { get; set; }

        /// <summary>
        /// The list of recipients for all groups for the user.
        /// </summary>
        /// <remarks>The order of recipients lines up with the order in <see cref="Groups"/>.</remarks>
        /// <example>
        /// If index 1 of <see cref="Groups"/> is the group with name "Test", then index 1 of <see
        /// cref="Recipients"/> would be the current user's recipient taken from a matching <see
        /// cref="UserGroup"/>. If index 2 was a group named "Test2", then index 2 of <see
        /// cref="Recipients"/> would be the user's recipient for that group. This is repeated for
        /// every group the user is in.
        /// </example>
        public List<User> Recipients { get; set; }
    }
}