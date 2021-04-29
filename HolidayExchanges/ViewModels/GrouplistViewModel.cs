using HolidayExchanges.Models;
using System.Collections.Generic;

namespace HolidayExchanges.ViewModels
{
    /// <summary>
    /// A viewmodel for a user's grouplist view.
    /// </summary>
    public class GrouplistViewModel
    {
        /// <summary>
        /// The user identifier whose groups will be listed.
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// The user's username
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The list of groups that the user is participating in.
        /// </summary>
        public List<Group> Groups { get; set; }

        /// <summary>
        /// The number of groups the user is in.
        /// </summary>
        public int Count
        {
            get => Groups.Count;
        }
    }
}