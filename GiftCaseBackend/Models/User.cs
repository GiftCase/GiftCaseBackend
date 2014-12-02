using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiftCaseBackend.Models
{
    public class Contact 
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public UserStatus Status { get; set; }

        public string ImageUrl { get; set; }

        public Gender Gender { get; set; }

        public Dictionary<string, int> Affinity { get; set; }

        public Contact()
        {
            UserName = "Anonymous";
            Status = UserStatus.NonRegistered;
            ImageUrl = "https://lh5.googleusercontent.com/-z4GINoMoCgA/AAAAAAAAAAI/AAAAAAAAABQ/CM0fRlsGcD8/photo.jpg";
            Affinity = new Dictionary<string, int>();
        }
    }

    public class User : Contact
    {
        public string FacebookAccessToken { get; set; }
        public List<Contact> Friends { get; set; }

        public List<Gift> SentGifts { get; set; }

        public List<Gift> ReceivedGifts { get; set; }
        

        public User()
        {
            Id = UserName;
        }
    }

    public enum UserStatus
    {
        Registered,
        NonRegistered,
        Banned
    }

    public enum Gender
    {
        Female, Male, Unknown
    }
}