using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HolidayExchanges.Models
{
    /// <summary>
    /// Represents a user's wish
    /// </summary>
    [Table("Wishes")]
    public class Wish
    {
        /// <summary>
        /// The wish identifier.
        /// </summary>
        [Key]
        public int WishID { get; set; }

        /// <summary>
        /// The user identifier of the user associated with the wish.
        /// </summary>
        [ForeignKey("User")]
        public int UserID { get; set; }

        /// <summary>
        /// The name of the item.
        /// </summary>
        [Display(Name = "Item Name")]
        [StringLength(100)]
        public string ItemName { get; set; }

        /// <summary>
        /// A short description of the item.
        /// </summary>
        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        /// <summary>
        /// The quantity of the item wanted.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// The link (url) to the item.
        /// </summary>
        [DataType(DataType.Url)]
        [Display(Name = "Item Link")]
        public string ItemLink { get; set; }

        /// <summary>
        /// Special purchasing instructions for the item (color, size, configurations, etc.).
        /// </summary>
        [Display(Name = "Special Instructions")]
        [DataType(DataType.MultilineText)]
        [StringLength(1000)]
        public string PurchasingInstructions { get; set; }

        /// <summary>
        /// Indicates if the item has been bought by the owner or any other user.
        /// </summary>
        [Display(Name = "Already Bought?")]
        public bool HasBeenBought { get; set; }

        /// <summary>
        /// The user associated with the wish.
        /// </summary>
        public virtual User User { get; set; }
    }
}