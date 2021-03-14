using HolidayExchanges.Models;
using System.Collections.Generic;

namespace HolidayExchanges.ViewModels
{
    public class UserViewModel
    {
        public User User { get; set; }
        public List<Group> Groups { get; set; }
        public List<User> Recipients { get; set; }
    }
}