using System.ComponentModel.DataAnnotations;

namespace HolidayExchanges.ViewModels
{
    public class WishlistSearchVM
    {
        [Display(Name = "Search Username")]
        public string SearchUsername { get; set; }

        [Display(Name = "Search Email")]
        public string SearchEmail { get; set; }
    }
}