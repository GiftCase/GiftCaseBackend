using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiftCaseBackend.Models
{
    public class TestRepository
    {
        public static List<ItemCategory> Categories = new List<ItemCategory>()
        {
            new ItemCategory(){Id=0,Name = "Book"},
            new ItemCategory(){Id=1,Name = "Movie"},
            new ItemCategory(){Id=2,Name = "Audio"},
        }; 

        public static List<Item> Items = new List<Item>()
        {
            new Item()
            {
                Category = Categories[0],
                LinkToTheStore = "http://www.amazon.co.uk/dp/1846573785 ",
                Name = "Fifty Shades of Grey",
                Store = Store.Amazon
            },
            new Item()
            {
                Category = Categories[0],
                LinkToTheStore = "http://www.amazon.co.uk/dp/1408855658 ",
                Name = "Harry Potter and the Philosopher's Stone",
                Store = Store.Amazon
            },
            new Item()
            {
                Category = Categories[1],
                LinkToTheStore = "http://www.amazon.co.uk/dp/B00F3TCF7O ",
                Name = "Captain America: The First Avenger",
                Store = Store.Amazon
            }
        };

        public static List<Contact> Friends = new List<Contact>()
        {
            new Contact()
            {
                UserName = "Ana Stepic",
                ImageUrl = "https://lh5.googleusercontent.com/-z4GINoMoCgA/AAAAAAAAAAI/AAAAAAAAABQ/CM0fRlsGcD8/photo.jpg",
                Status = UserStatus.NonRegistered
            },
            new Contact()
            {
                UserName = "Damir Tomic",
                ImageUrl = "https://lh5.googleusercontent.com/-z4GINoMoCgA/AAAAAAAAAAI/AAAAAAAAABQ/CM0fRlsGcD8/photo.jpg",
                Status = UserStatus.Registered
            },
            new Contact()
            {
                UserName = "Vlatko",
                ImageUrl = "https://lh5.googleusercontent.com/-z4GINoMoCgA/AAAAAAAAAAI/AAAAAAAAABQ/CM0fRlsGcD8/photo.jpg",
                Status = UserStatus.Registered
            },
            new Contact()
            {
                UserName = "Gijs",
                ImageUrl = "https://lh5.googleusercontent.com/-z4GINoMoCgA/AAAAAAAAAAI/AAAAAAAAABQ/CM0fRlsGcD8/photo.jpg",
                Status = UserStatus.Registered
            },
        };

        public static List<Gift> Gifts = new List<Gift>()
        {
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
        };

        public static List<GiftcaseEvent> Events = new List<GiftcaseEvent>()
        {
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
        }; 
    }
}