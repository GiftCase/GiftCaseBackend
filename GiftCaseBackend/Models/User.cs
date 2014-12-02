﻿using System;
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

        public List<GiftcaseEvent> EventList = new List<GiftcaseEvent>();

        public Contact()
        {
            UserName = "Anonymous";
            Status = UserStatus.NonRegistered;
            ImageUrl = "https://lh5.googleusercontent.com/-z4GINoMoCgA/AAAAAAAAAAI/AAAAAAAAABQ/CM0fRlsGcD8/photo.jpg";
            Affinity = new Dictionary<string, int>();
            Gender = Gender.Unknown;

            Affinity.Add(TestRepository.ItemCategoryEnum.Game.ToString(), 10); //default affinities, remove when facebook API starts working
            Affinity.Add(TestRepository.ItemCategoryEnum.Book.ToString(), 2);
            Affinity.Add(TestRepository.ItemCategoryEnum.Audio.ToString(), 0);
            Affinity.Add(TestRepository.ItemCategoryEnum.Movie.ToString(), 0);

            EventList.Add(TestRepository.Events[1]); 

        }

        

    }

    public class User : Contact
    {
        public string FacebookAccessToken { get; set; }
        public List<Contact> Friends { get; set; }

        public List<Gift> SentGifts { get; set; }

        public List<Gift> ReceivedGifts { get; set; }

        public string TelcoID { get; set; } //we assume all of our users have telcoID

        


        public User()
        {
            Id = UserName;
        }

        public void CalculateAffinity()
        {
            //this.Affinity =  //i should probably improve this!
            GiftRecommendationEngine.CalculateAffinity(this);
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