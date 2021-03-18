using HolidayExchanges.Models;
using System.Collections.Generic;

namespace HolidayExchanges.ViewModels
{
    public class WishlistViewModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public List<Wish> Wishlist { get; set; }

        public int Count
        {
            get => Wishlist.Count;
        }
    }
}