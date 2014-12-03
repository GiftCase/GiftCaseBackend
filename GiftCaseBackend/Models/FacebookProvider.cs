using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using HtmlAgilityPack;
using Newtonsoft.Json;
using SimpleJSON;

namespace GiftCaseBackend.Models
{
    public class FacebookProvider
    {
        public static void UpdateUserInfoWithPublicProfile(User user)
        { 
            try
            {
                FacebookPublicProfile profile = null;

                string requestUrl = "https://graph.facebook.com/" + user.Id;
                // obtain the public profile data
                var request = WebRequest.CreateHttp(requestUrl);
                var stream = request.GetResponse().GetResponseStream();
                var reader = new StreamReader(stream);
                var result = reader.ReadToEnd();

                profile = JsonConvert.DeserializeObject<FacebookPublicProfile>(result);

                if (profile.gender == "female")
                    user.Gender = Gender.Female;
                else if (profile.gender == "male")
                    user.Gender = Gender.Male;
                else user.Gender = Gender.Unknown;

                user.Name = profile.name;
                user.UserName = profile.username;


            }
            catch (Exception e)
            {
               
            }
        }

        static void CountBooks(User user) //token as a 2nd argument. Or should that be in users?
        {
            string AnaLongTermToken = "CAAMewqUUav0BADBpQbA3mQZAwzZB1mmL2TzR7hrYildnnEHJUCipZC0QZAZAZCoKhwh6ZAHd80tCYSMIhluo6IeRBlkSctEK7ZAHHff7OnVPRe1hjTRW0FPsmbitIYtbCZC8Gj7bCfG39Lqv63ACaSs7TTSsd2p725c5LthCUwp4qA3pdZACWIqLDOfmKtcZCCHCrCIRuVknu2Ru4ZBuqAu1lajO";

            string requestUrl = "https://graph.facebook.com/" + user.Id + "/books?access_token=" + AnaLongTermToken; //likes, music,games, movies, television, books
            // obtain the public profile data
            var request = WebRequest.CreateHttp(requestUrl);
            var stream = request.GetResponse().GetResponseStream();
            var reader = new StreamReader(stream);
            var result = reader.ReadToEnd();

            //profile = JsonConvert.DeserializeObject<FacebookPublicProfile>(result);
            dynamic JSONReponse = JsonConvert.DeserializeObject(result);

            string[] grupa = new String[100];
            int i = 0;

            foreach (dynamic Stvar in JSONReponse.data)
            {
               // grupa[i] = Stvar.name;
                ++i;
            }

            user.Affinity[TestRepository.ItemCategoryEnum.Book.ToString()] = i;
        }

        static void CountGames(User user) //token as a 2nd argument. Or should that be in users?
        {
            string AnaLongTermToken = "CAAMewqUUav0BADBpQbA3mQZAwzZB1mmL2TzR7hrYildnnEHJUCipZC0QZAZAZCoKhwh6ZAHd80tCYSMIhluo6IeRBlkSctEK7ZAHHff7OnVPRe1hjTRW0FPsmbitIYtbCZC8Gj7bCfG39Lqv63ACaSs7TTSsd2p725c5LthCUwp4qA3pdZACWIqLDOfmKtcZCCHCrCIRuVknu2Ru4ZBuqAu1lajO";

            string requestUrl = "https://graph.facebook.com/" + user.Id + "/games?access_token=" + AnaLongTermToken; //likes, music,games, movies, television, books
            // obtain the public profile data
            var request = WebRequest.CreateHttp(requestUrl);
            var stream = request.GetResponse().GetResponseStream();
            var reader = new StreamReader(stream);
            var result = reader.ReadToEnd();

            //profile = JsonConvert.DeserializeObject<FacebookPublicProfile>(result);
            dynamic JSONReponse = JsonConvert.DeserializeObject(result);

            string[] grupa = new String[100];
            int i = 0;

            foreach (dynamic Stvar in JSONReponse.data)
            {
                // grupa[i] = Stvar.name;
                ++i;
            }

            user.Affinity[TestRepository.ItemCategoryEnum.Game.ToString()] = i;
        }

        static void CountMovies(User user) //token as a 2nd argument. Or should that be in users?
        {
            string AnaLongTermToken = "CAAMewqUUav0BADBpQbA3mQZAwzZB1mmL2TzR7hrYildnnEHJUCipZC0QZAZAZCoKhwh6ZAHd80tCYSMIhluo6IeRBlkSctEK7ZAHHff7OnVPRe1hjTRW0FPsmbitIYtbCZC8Gj7bCfG39Lqv63ACaSs7TTSsd2p725c5LthCUwp4qA3pdZACWIqLDOfmKtcZCCHCrCIRuVknu2Ru4ZBuqAu1lajO";

            string requestUrl = "https://graph.facebook.com/" + user.Id + "/movies?access_token=" + AnaLongTermToken; //likes, music,games, movies, television, books
            // obtain the public profile data
            var request = WebRequest.CreateHttp(requestUrl);
            var stream = request.GetResponse().GetResponseStream();
            var reader = new StreamReader(stream);
            var result = reader.ReadToEnd();

            //profile = JsonConvert.DeserializeObject<FacebookPublicProfile>(result);
            dynamic JSONReponse = JsonConvert.DeserializeObject(result);

            string[] grupa = new String[100];
            int i = 0;

            foreach (dynamic Stvar in JSONReponse.data)
            {
                // grupa[i] = Stvar.name;
                ++i;
            }

            user.Affinity[TestRepository.ItemCategoryEnum.Movie.ToString()] = i;
        }

        static void CountMusic(User user) //token as a 2nd argument. Or should that be in users?
        {
            string AnaLongTermToken = "CAAMewqUUav0BADBpQbA3mQZAwzZB1mmL2TzR7hrYildnnEHJUCipZC0QZAZAZCoKhwh6ZAHd80tCYSMIhluo6IeRBlkSctEK7ZAHHff7OnVPRe1hjTRW0FPsmbitIYtbCZC8Gj7bCfG39Lqv63ACaSs7TTSsd2p725c5LthCUwp4qA3pdZACWIqLDOfmKtcZCCHCrCIRuVknu2Ru4ZBuqAu1lajO";

            string requestUrl = "https://graph.facebook.com/" + user.Id + "/music?access_token=" + AnaLongTermToken; //likes, music,games, movies, television, books
            // obtain the public profile data
            var request = WebRequest.CreateHttp(requestUrl);
            var stream = request.GetResponse().GetResponseStream();
            var reader = new StreamReader(stream);
            var result = reader.ReadToEnd();

            //profile = JsonConvert.DeserializeObject<FacebookPublicProfile>(result);
            dynamic JSONReponse = JsonConvert.DeserializeObject(result);

            string[] grupa = new String[100];
            int i = 0;

            foreach (dynamic Stvar in JSONReponse.data)
            {
                // grupa[i] = Stvar.name;
                ++i;
            }

            user.Affinity[TestRepository.ItemCategoryEnum.Audio.ToString()] = i;
        }



        public static void UpdateAffinity(User user)
        {

            string AnaLongTermToken = "CAAMewqUUav0BADBpQbA3mQZAwzZB1mmL2TzR7hrYildnnEHJUCipZC0QZAZAZCoKhwh6ZAHd80tCYSMIhluo6IeRBlkSctEK7ZAHHff7OnVPRe1hjTRW0FPsmbitIYtbCZC8Gj7bCfG39Lqv63ACaSs7TTSsd2p725c5LthCUwp4qA3pdZACWIqLDOfmKtcZCCHCrCIRuVknu2Ru4ZBuqAu1lajO";

            try
            {
                CountBooks(user);
                CountGames(user);
                CountMovies(user);
                CountMusic(user);

            }
            catch (Exception e)
            {
               
            }
        }

    }



    public class FacebookPublicProfile
    {
        public string id;
        public string first_name;
        public string gender;
        public string last_name;
        public string link;
        public string name;
        public string username;
    }
}