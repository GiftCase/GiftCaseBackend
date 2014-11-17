using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiftCaseBackend.Models
{
    public class TestRepository
    {
        public static List<Item> Gifts = new List<Item>()
        {
            new Item()
            {
                Category = ItemCategory.Book,
                LinkToTheStore = "http://www.amazon.co.uk/dp/1846573785 ",
                Name = "Fifty Shades of Grey",
                Store = Store.Amazon
            },
            new Item()
            {
                Category = ItemCategory.Book,
                LinkToTheStore = "http://www.amazon.co.uk/dp/1408855658 ",
                Name = "Harry Potter and the Philosopher's Stone",
                Store = Store.Amazon
            },
            new Item()
            {
                Category = ItemCategory.Movie,
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
            }
        }; 
    }
}