using HolidayExchanges.Models;
using System.Collections.Generic;

namespace HolidayExchanges.ViewModels
{
    /// <summary>
    /// A viewmodel for the group details page.
    /// </summary>
    public class GroupViewModel
    {
        /// <summary>
        /// The group being viewed.
        /// </summary>
        public Group Group { get; set; }

        /// <summary>
        /// The list of users that are in the group.
        /// </summary>
        public List<User> Users { get; set; }

        /// <summary>
        /// Indicates if the current viewing user is the group's creator.
        /// </summary>
        public bool IsCreator { get; set; }
    }
}