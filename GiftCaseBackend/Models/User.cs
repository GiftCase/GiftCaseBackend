using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiftCaseBackend.Models
{
    public class Contact
    {
        public string UserName { get; set; }

        public UserStatus Status { get; set; }

        public string ImageUrl { get; set; }
    }

    public class User : Contact
    {
        public string Id { get; set; }
        public string FacebookAccessToken { get; set; }
        public List<Contact> Friends { get; set; }

        public List<Gift> SentGifts { get; set; }

        public List<Gift> ReceivedGifts { get; set; } 
    }

    public enum UserStatus
    {
        Registered,
        NonRegistered,
        Banned
    }
}