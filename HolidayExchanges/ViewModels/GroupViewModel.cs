using HolidayExchanges.Models;
using System.Collections.Generic;

namespace HolidayExchanges.ViewModels
{
    public class GroupViewModel
    {
        public Group Group { get; set; }
        public List<User> Users { get; set; }
        public bool IsCreator { get; set; }
    }
}