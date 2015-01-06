using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiftCaseBackend.Models
{
    public class TestRepository
    {
        public enum ItemCategoryEnum
        {
            Audio,
            Book,
            Game,
            Video
        }

        public static Dictionary<string,ItemCategory> Categories = new Dictionary<string,ItemCategory>()
        {
            
            {ItemCategoryEnum.Book.ToString(),new ItemCategory(){Id=1,Name = ItemCategoryEnum.Book.ToString()}}, // amazon browse node id = 1000
            {ItemCategoryEnum.Video.ToString(),new ItemCategory(){Id=2,Name = ItemCategoryEnum.Video.ToString()}},//amazon browse node id = 130
            {ItemCategoryEnum.Audio.ToString(),new ItemCategory(){Id=4,Name = ItemCategoryEnum.Audio.ToString()}},


            {ItemCategoryEnum.Game.ToString(),new ItemCategory(){Id=3,Name = ItemCategoryEnum.Game.ToString()}},
            {"Action",new ItemCategory(){Id=19,Name = "Action",ParentCategory = 3}},
            {"Indie",new ItemCategory(){Id=492,Name = "Indie",ParentCategory = 3}},
            {"Adventure",new ItemCategory(){Id=21,Name = "Adventure",ParentCategory = 3}},
            {"RPG",new ItemCategory(){Id=122,Name = "RPG",ParentCategory = 3}},
            {"Simulation",new ItemCategory(){Id=599,Name = "Simulation",ParentCategory = 3}},
            {"Casual",new ItemCategory(){Id=597,Name = "Casual",ParentCategory = 3}},
            {"SinglePlayer",new ItemCategory(){Id=4182,Name = "SinglePlayer",ParentCategory = 3}},
            {"MMO",new ItemCategory(){Id=128,Name = "MMO",ParentCategory = 3}},
            {"MultiPlayer",new ItemCategory(){Id=3859,Name = "MultiPlayer",ParentCategory = 3}},
            {"Racing",new ItemCategory(){Id=699,Name = "Racing",ParentCategory = 3}},
            {"Sports",new ItemCategory(){Id=701,Name = "Sports",ParentCategory = 3}},
            {"Shooter",new ItemCategory(){Id=1774,Name = "Shooter",ParentCategory = 3}},
            {"FPS",new ItemCategory(){Id=1663,Name = "FPS",ParentCategory = 3}},
            {"SciFi",new ItemCategory(){Id=3942,Name = "SciFi",ParentCategory = 3}},
            {"Strategy",new ItemCategory(){Id=9,Name = "Strategy",ParentCategory = 3}},
        };
        public static List<Item> Items = new List<Item>()
        {
            new Book()
            {
                Category = Categories[ItemCategoryEnum.Book.ToString()],
                LinkToTheStore = "http://www.amazon.co.uk/dp/1846573785 ",
                Name = "Fifty Shades of Grey",
                Store = Store.Amazon,
                Id = "0",
                Price = 8.6f
            },
            new Book()
            {
                Category = Categories[ItemCategoryEnum.Book.ToString()],
                LinkToTheStore = "http://www.amazon.co.uk/dp/1408855658 ",
                Name = "Harry Potter and the Philosopher's Stone",
                Store = Store.Amazon,
                Id = "1",
                Price = 18.6f
            },
            new Video()
            {
                Category = Categories[ItemCategoryEnum.Video.ToString()],
                LinkToTheStore = "http://www.amazon.co.uk/dp/B00F3TCF7O ",
                Name = "Captain America: The First Avenger",
                Store = Store.Amazon,
                Id = "2",
                Price = 5.55f
            } 
        };

        public static List<Contact> Friends = new List<Contact>()
        {
            /*
            new Contact()
            {
                UserName = "Ana Stepic",
                Id = "10152464438050382",
                ImageUrl = "https://lh5.googleusercontent.com/-z4GINoMoCgA/AAAAAAAAAAI/AAAAAAAAABQ/CM0fRlsGcD8/photo.jpg",
                Status = UserStatus.NonRegistered,
            },
            new Contact()
            {
                UserName = "Damir Tomic",
                Id = "10204523203015435",
                ImageUrl = "https://lh5.googleusercontent.com/-z4GINoMoCgA/AAAAAAAAAAI/AAAAAAAAABQ/CM0fRlsGcD8/photo.jpg",
                Status = UserStatus.Registered,
            },
            new Contact()
            {
                UserName = "Vlatko Klabučar",
                Id = "10152479696077544",
                ImageUrl = "https://lh5.googleusercontent.com/-z4GINoMoCgA/AAAAAAAAAAI/AAAAAAAAABQ/CM0fRlsGcD8/photo.jpg",
                Status = UserStatus.Registered,
            },
            new Contact()
            {
                UserName = "Gijs",
                ImageUrl = "https://lh5.googleusercontent.com/-z4GINoMoCgA/AAAAAAAAAAI/AAAAAAAAABQ/CM0fRlsGcD8/photo.jpg",
                Status = UserStatus.Registered,
            },
             */ 
        };

        public static List<Gift> Gifts = new List<Gift>()
        {
            /*
            new Gift(){DateOfPurchase = new DateTime(2014,4,20,12,22,15,0), Item = Items[0], Status = GiftStatus.Received, 
                UserWhoGaveTheGift = Friends[1], UserWhoReceivedTheGift = Friends[2]},
            new Gift()
            {
                DateOfPurchase = new DateTime(2014,5,22,13,12,5,0), Item = Items[1], Status = GiftStatus.Claimed,
                UserWhoGaveTheGift = Friends[2], UserWhoReceivedTheGift = Friends[1]
            },
            new Gift(){DateOfPurchase = new DateTime(2014,5,22,13,12,5,0), Item = Items[2], Status = GiftStatus.NotReceivedYet,
                UserWhoGaveTheGift = Friends[3], UserWhoReceivedTheGift = Friends[1]},
            new Gift(){DateOfPurchase = new DateTime(2014,5,22,13,12,5,0), Item = Items[1], Status = GiftStatus.NotReceivedYet,
                UserWhoGaveTheGift = Friends[3], UserWhoReceivedTheGift = Friends[2]},
            new Gift(){DateOfPurchase = new DateTime(2014,5,22,13,12,5,0), Item = Items[0], Status = GiftStatus.NotReceivedYet,
                UserWhoGaveTheGift = Friends[2], UserWhoReceivedTheGift = Friends[1]},
            new Gift(){DateOfPurchase = new DateTime(2014,5,22,13,12,5,0), Item = Items[1], Status = GiftStatus.NotReceivedYet,
                UserWhoGaveTheGift = Friends[1], UserWhoReceivedTheGift = Friends[2]},
                new Gift(){DateOfPurchase = new DateTime(2014,5,22,13,12,5,0), Item = Items[1], Status = GiftStatus.NotReceivedYet,
                UserWhoGaveTheGift = Friends[3], UserWhoReceivedTheGift = Friends[0]},
            new Gift(){DateOfPurchase = new DateTime(2014,5,22,13,12,5,0), Item = Items[0], Status = GiftStatus.NotReceivedYet,
                UserWhoGaveTheGift = Friends[0], UserWhoReceivedTheGift = Friends[1]},
            new Gift(){DateOfPurchase = new DateTime(2014,5,22,13,12,5,0), Item = Items[1], Status = GiftStatus.NotReceivedYet,
                UserWhoGaveTheGift = Friends[1], UserWhoReceivedTheGift = Friends[0]},
             */
        };

        public static List<GiftcaseEvent> Events = new List<GiftcaseEvent>()
        {
            /*
            new GiftcaseEvent()
            {
                Date = new DateTime(2016,5,23), Type = GiftcaseEventType.Anniversary, RelatedContacts = new List<Contact>(){Friends[0]},
                Details = "Some anniversary..."
            },
            new GiftcaseEvent()
            {
                Date = new DateTime(2016,4,3), Type = GiftcaseEventType.Birthday, RelatedContacts = new List<Contact>(){Friends[1]},
                Details = "Birthday...not..."
            },
            new GiftcaseEvent()
            {
                Date = new DateTime(2016,6,23), Type = GiftcaseEventType.Graduation, RelatedContacts = new List<Contact>(){Friends[2], Friends[1], Friends[0]},
                Details = "Hopefully :)"
            },
             */ 
        }; 
    }
}