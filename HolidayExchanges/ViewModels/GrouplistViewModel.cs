using HolidayExchanges.Models;
using System.Collections.Generic;

namespace HolidayExchanges.ViewModels
{
    public class GrouplistViewModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public List<Group> Groups { get; set; }

        public int Count
        {
            get => Groups.Count;
        }
    }
}