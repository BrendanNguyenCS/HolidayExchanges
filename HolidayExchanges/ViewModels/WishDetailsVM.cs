using System.ComponentModel.DataAnnotations;

namespace HolidayExchanges.ViewModels
{
    /// <summary>
    /// A viewmodel for the wish details page.
    /// </summary>
    public class WishDetailsVM
    {
        /// <summary>
        /// Empty constructor.
        /// </summary>
        public WishDetailsVM() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WishDetailsVM"/> viewmodel.
        /// </summary>
        /// <param name="id"></param>
        public WishDetailsVM(int? id)
        {
            WishID = id.Value;
        }

        /// <summary>
        /// The username of the wish owner.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The wish identifier.
        /// </summary>
        public int WishID { get; set; }

        /// <summary>
        /// The user identifier.
        /// </summary>
        public int UserID { get; set; }

        public bool PageOwner { get; set; }

        #region Wish Info

        /// <summary>
        /// The wish item name.
        /// </summary>
        [Display(Name = "Item Name")]
        [StringLength(100)]
        public string ItemName { get; set; }

        /// <summary>
        /// The wish item description.
        /// </summary>
        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        /// <summary>
        /// The wish item quantity.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// The wish item link.
        /// </summary>
        [DataType(DataType.Url)]
        [Display(Name = "Item Link")]
        public string ItemLink { get; set; }

        /// <summary>
        /// The special instructions for the wish item.
        /// </summary>
        [Display(Name = "Special Instructions")]
        [DataType(DataType.MultilineText)]
        [StringLength(1000)]
        public string PurchasingInstructions { get; set; }

        /// <summary>
        /// Condition if the item has been bought by someone already.
        /// </summary>
        [Display(Name = "Already Bought?")]
        public bool HasBeenBought { get; set; }

        #endregion Wish Info
    }
}