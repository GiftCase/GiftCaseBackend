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

        public UserStatus Status { get; set; }

        public string ImageUrl { get; set; }

        public Contact()
        {
            UserName = "Anonymous";
            Status = UserStatus.NonRegistered;
            ImageUrl = "https://lh5.googleusercontent.com/-z4GINoMoCgA/AAAAAAAAAAI/AAAAAAAAABQ/CM0fRlsGcD8/photo.jpg";
        }
    }

    public class User : Contact
    {
        public string FacebookAccessToken { get; set; }

        public List<Contact> Friends { get; set; }

        public List<Gift> SentGifts { get; set; }

        public List<Gift> ReceivedGifts { get; set; }

        public Dictionary<string, int> Affinity = new Dictionary<string, int>();

        public string TelcoID { get; set; } //we assume all of our users have telcoID

        public UserGender Gender = UserGender.Unspecified;

        public List<GiftcaseEvent> EventList = new List<GiftcaseEvent>();


        public User()
        {
            Id = UserName;
            Affinity.Add(ItemCategoryEnum.Game.ToString(), 10); //default affinities, remove when facebook API starts working
            Affinity.Add(ItemCategoryEnum.Book.ToString(), 2);
            Affinity.Add(ItemCategoryEnum.Audio.ToString(), 0);
            Affinity.Add(ItemCategoryEnum.Movie.ToString(),0);

            EventList.Add(TestRepository.Events[1]); 

        }
    }

    public enum UserStatus
    {
        Registered,
        NonRegistered,
        Banned
    }

    public enum UserGender
    {
        Male,
        Female,
        Unspecified
    }

}