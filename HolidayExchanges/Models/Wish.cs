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
        /// Gets or sets the wish identifier.
        /// </summary>
        /// <value>The wish identifier.</value>
        [Key]
        public int WishID { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>The user identifier.</value>
        [ForeignKey("User")]
        public int UserID { get; set; }

        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        /// <value>The name of the item.</value>
        [Display(Name = "Item Name")]
        [StringLength(100)]
        public string ItemName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>The quantity.</value>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the item link.
        /// </summary>
        /// <value>The item link.</value>
        [DataType(DataType.Url)]
        [Display(Name = "Item Link")]
        public string ItemLink { get; set; }

        /// <summary>
        /// Gets or sets the purchasing instructions.
        /// </summary>
        /// <value>The purchasing instructions.</value>
        [Display(Name = "Special Instructions")]
        [DataType(DataType.MultilineText)]
        [StringLength(1000)]
        public string PurchasingInstructions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has been bought.
        /// </summary>
        /// <value><c>true</c> if this instance has been bought; otherwise, <c>false</c>.</value>
        [Display(Name = "Already Bought?")]
        public bool HasBeenBought { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>The user.</value>
        public virtual User User { get; set; }
    }
}