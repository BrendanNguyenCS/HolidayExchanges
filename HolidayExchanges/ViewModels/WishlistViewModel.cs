using HolidayExchanges.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HolidayExchanges.ViewModels
{
    public class WishlistViewModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public List<Wish> Wishlist { get; set; }

        [Display(Name = "Username")]
        public string SearchUserName { get; set; }

        [Display(Name = "Email")]
        public string SearchEmail { get; set; }

        public int Count
        {
            get => Wishlist != null ? Wishlist.Count : 0;
        }
    }
}