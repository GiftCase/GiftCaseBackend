using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiftCaseBackend.Models
{
    public class ItemCategory
    {
        public int Id;
        
        public string Name;

        public int? ParentCategory;
    }

    public class Item
    {
        public string Name { get; set; }

        public string LinkToTheStore { get; set; }

        public ItemCategory Category { get; set; }

        public Store Store { get; set; }
    }
    public class Gift
    {
        public Item Item { get; set; }

        public GiftStatus Status { get; set; }

        public DateTime DateOfPurchase { get; set; }

        public Contact UserWhoGaveTheGift { get; set; }

        public Contact UserWhoReceivedTheGift { get; set; }
 
    }

    public enum GiftStatus
    {
        NotGiven, // this gift has not been given yet
        NotReceivedYet, // gift has been sent but the user getting it has not yet seen it
        Received, // the user getting the gift has seen the gift in his app but has not claimed it yet
        Claimed // user downloaded/claimed the gift he has received
    }
    /*
    public enum ItemCategory
    {
        Music,
        Book,
        Movie,
    }
    */
    public enum Store
    {
        Amazon,
        eBay,
        iTunes,
    }
}