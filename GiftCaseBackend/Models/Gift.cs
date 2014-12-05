using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
        public string Id { get; set; }

        public Store Store { get; set; }
        
        public string Name { get; set; }

        public string LinkToTheStore { get; set; }

        public ItemCategory Category { get; set; }

        

        public string Description { get; set; }

        public string IconUrl { get; set; }

        public float Price { get; set; }

        public float PreviousPrice { get; set; }

        public string PriceCurrency { get; set; } 

        public Item()
        {
            Id = "0";
            Name = "Untitled";
            Category = TestRepository.Categories[TestRepository.ItemCategoryEnum.Book.ToString()];
            Store = Store.Amazon;
            Description = "No description";
            PriceCurrency = "$";
        }
    }

    [KnownType(typeof(GiftCaseBackend.Models.Book))]
    public class Book : Item
    {
        public string Author { get; set; }

        public Book()
        {
            Author = "Unknown";
        }
    }

    [KnownType(typeof(GiftCaseBackend.Models.Music))]
    public class Music : Item
    {
        public string Artist { get; set; }

        public Music()
        {
            Artist = "Unknown";
        }
    }
    [KnownType(typeof(GiftCaseBackend.Models.Video))]
    public class Video : Item
    {
        public string Director { get; set; }

        public Video()
        {
            Director = "Unknown";
        }
    }
    [KnownType(typeof(GiftCaseBackend.Models.Game))]
    public class Game : Item
    {
        public string Platform { get; set; }

        public Game()
        {
            Platform = "PC";
        }
    }

    public class Gift
    {
        public Item Item { get; set; }

        public GiftStatus Status { get; set; }

        public DateTime DateOfPurchase { get; set; }

        public Contact UserWhoGaveTheGift { get; set; }

        public Contact UserWhoReceivedTheGift { get; set; }

        public ShortGift Shorten()
        {
            ShortGift gift = new ShortGift();
            gift.ItemId = Item.Id;
            gift.Store = Item.Store;
            gift.Status = Status;
            gift.DateOfPurchase = DateOfPurchase;
            gift.IdOfUserWhoGaveTheGift = UserWhoGaveTheGift.Id;
            gift.IdOfUserWhoReceivedTheGift = UserWhoReceivedTheGift.Id;
            return gift;
        }
 
    }

    /// <summary>
    /// 
    /// </summary>
    public class ShortGift
    {
        public string ItemId { get; set; }

        public Store Store { get; set; }

        public GiftStatus Status { get; set; }

        public DateTime DateOfPurchase { get; set; }

        public string IdOfUserWhoGaveTheGift { get; set; }

        public string IdOfUserWhoReceivedTheGift { get; set; }

        public static explicit operator ShortGift(Gift originalGift)  // explicit byte to digit conversion operator
        {
            ShortGift gift = new ShortGift();
            gift.ItemId = originalGift.Item.Id;
            gift.Store = originalGift.Item.Store;
            gift.Status = originalGift.Status;
            gift.DateOfPurchase = originalGift.DateOfPurchase;
            gift.IdOfUserWhoGaveTheGift = originalGift.UserWhoGaveTheGift.Id;
            gift.IdOfUserWhoReceivedTheGift = originalGift.UserWhoReceivedTheGift.Id;
            return gift;
        }

    }

    public enum GiftStatus
    {
        NotGiven, // this gift has not been given yet
        NotReceivedYet, // gift has been sent but the user getting it has not yet seen it
        Received, // the user getting the gift has seen the gift in his app but has not claimed it yet
        Claimed // user downloaded/claimed the gift he has received
    }

    public enum Store
    {
        Amazon,
        eBay,
        iTunes,
        Steam
    }

    
}