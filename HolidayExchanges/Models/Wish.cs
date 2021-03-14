﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HolidayExchanges.Models
{
    [Table("Wishes")]
    public class Wish
    {
        [Key]
        public int WishID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        [Display(Name = "Item Name")]
        [StringLength(100)]
        public string ItemName { get; set; }

        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public int Quantity { get; set; }

        [DataType(DataType.Url)]
        [Display(Name = "Item Link")]
        public string ItemLink { get; set; }

        [Display(Name = "Special Instructions")]
        [DataType(DataType.MultilineText)]
        [StringLength(1000)]
        public string PurchasingInstructions { get; set; }

        [Display(Name = "Already Bought?")]
        public bool HasBeenBought { get; set; }

        public virtual User User { get; set; }
    }
}