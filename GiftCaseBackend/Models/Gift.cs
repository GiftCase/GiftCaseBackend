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
        public int Id { get; set; }
        public string Name { get; set; }

        public string LinkToTheStore { get; set; }

        public ItemCategory Category { get; set; }

        public Store Store { get; set; }

        public string Description { get; set; }

        public string IconUrl { get; set; }

        public float Price { get; set; }

        public float PreviousPrice { get; set; }

        public string PriceCurrency { get; set; }

        public Item()
        {
            Id = 0;
            Name = "Untitled";
            Category = new ItemCategory();
            Category = TestRepository.Categories[0];
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
    [KnownType(typeof(GiftCaseBackend.Models.Movie))]
    public class Movie : Item
    {
        public string Studio { get; set; }

        public Movie()
        {
            Studio = "Unknown";
        }
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

    public enum Store
    {
        Amazon,
        eBay,
        iTunes,
    }
}