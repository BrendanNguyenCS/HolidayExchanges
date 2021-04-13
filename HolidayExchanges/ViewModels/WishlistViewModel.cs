using HolidayExchanges.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HolidayExchanges.ViewModels
{
    public class WishlistViewModel
    {
        public WishlistViewModel()
        {
            Wishlist = new List<Wish>();
        }

        /// <summary>
        /// The identifier for the associated user.
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// The username for the associated user.
        /// </summary>
        [Display(Name = "Username")]
        public string UserName { get; set; }

        /// <summary>
        /// The user's wishlist.
        /// </summary>
        public List<Wish> Wishlist { get; set; }

        /// <summary>
        /// A read-only property for the amount of wishes in wishlist.
        /// </summary>
        public int Count
        {
            get => Wishlist.Count;
        }
    }
}