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

        public static string DamirLongTermExtendedToken = "CAAMewqUUav0BADZAGXgZAmFhcNhjhtu6ZBcjlB1TMtDgJ7IOe6ZBKgcJUeVcT4vaoBWLTQvydLk0wKD1r2hGWTQOI8GllpQGTorI4oQ4p0QtYXSJMTGjkt9pOyFA0VPiIizKWnmIaM7tqkjd0WykCJjKtBNnwN1wQTgr0tVGCQ1NgBjWnhUe";

        public static string FetchExtendedToken(User user, string shortToken)
        {

            

            string AppKey = "878246272199421";
            string SecretAppKey = "3797d105cf3f7ed9b4142473f7727d24";
            string requestUrl = "https://graph.facebook.com/oauth/access_token?grant_type=fb_exchange_token&client_id="+AppKey+"&client_secret="+SecretAppKey+"&fb_exchange_token="+shortToken;

            var request = WebRequest.CreateHttp(requestUrl);
            var stream = request.GetResponse().GetResponseStream();
            var reader = new StreamReader(stream);
            var result = reader.ReadToEnd();

            string s = result.Split('=')[1].Split('&')[0];
            //dynamic JSONReponse = JsonConvert.DeserializeObject(result); //if no events, this is null!
            //dynamic JSONResponse = JsonConvert.DeserializeXmlNode(result);

            //JSONReponse.data.access_token ????   
            
            user.ExtendedToken = s;
            //UNFINISHED!!!
            //throw new NotImplementedException("I need more tokens to test. I don't want to mess up the one I already have! XD");
            return "";

        }

        public static void UpdateUserInfoWithPublicProfile(User user)
        { 
            try
            {
                FacebookPublicProfile profile = null;

                string requestUrl = "https://graph.facebook.com/me?access_token=" + user.FacebookAccessToken;
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

                // get picture
                //https://graph.facebook.com/10152464438050382/picture?redirect=false&access_token=
                requestUrl = "https://graph.facebook.com/"+user.Id+"/picture?redirect=false&access_token="+user.FacebookAccessToken;
                request = WebRequest.CreateHttp(requestUrl);
                stream = request.GetResponse().GetResponseStream();
                reader = new StreamReader(stream);
                result = reader.ReadToEnd();

                dynamic pictureData = JsonConvert.DeserializeObject(result);
                user.ImageUrl = pictureData.data.url;

                FacebookProvider.FetchExtendedToken(user, user.FacebookAccessToken);

            }
            catch (Exception e)
            {
                // if this failed, access token is not valid, try to fetch public profile with public user id
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
                catch (Exception e2)
                {
                }
            }

            

        }

        static public void FetchEvents(User user)
        {
            try
            {
                string AnaLongTermToken = "CAAMewqUUav0BADBpQbA3mQZAwzZB1mmL2TzR7hrYildnnEHJUCipZC0QZAZAZCoKhwh6ZAHd80tCYSMIhluo6IeRBlkSctEK7ZAHHff7OnVPRe1hjTRW0FPsmbitIYtbCZC8Gj7bCfG39Lqv63ACaSs7TTSsd2p725c5LthCUwp4qA3pdZACWIqLDOfmKtcZCCHCrCIRuVknu2Ru4ZBuqAu1lajO";

                string requestUrl = "https://graph.facebook.com/" + user.Id + "/events?access_token=" + AnaLongTermToken; //likes, music,games, movies, television, books
                // obtain the public profile data
                var request = WebRequest.CreateHttp(requestUrl);
                var stream = request.GetResponse().GetResponseStream();
                var reader = new StreamReader(stream);
                var result = reader.ReadToEnd();

                dynamic JSONReponse = JsonConvert.DeserializeObject(result); //if no events, this is null!

                string[] grupa = new String[100];
                int i = 0;

                foreach (dynamic Stvar in JSONReponse.data)
                {
                    TestRepository.Events.Add(new GiftcaseEvent()
                    {
                        Date = new DateTime(2016, 5, 23), //Stvar.start_time
                        Type = GiftcaseEventType.Anniversary,
                        RelatedContacts = new List<Contact>() { TestRepository.Friends[0] },
                        Details = Stvar.name
                    });
                    // grupa[i] = Stvar.name;
                    ++i;
                    
                }

            }
            catch (Exception e)
            {
                
                //throw;
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

            /*
            string[] grupa = new String[100];
            int i = 0;

            foreach (dynamic Stvar in JSONReponse.data)
            {
               // grupa[i] = Stvar.name;
                ++i;
            }
            */

            int n = JSONReponse.data.Count;


            user.Affinity[TestRepository.ItemCategoryEnum.Book.ToString()] = n;
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

            int n = JSONReponse.data.Count;

            user.Affinity[TestRepository.ItemCategoryEnum.Game.ToString()] = n;
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

            int n = JSONReponse.data.Count;


            user.Affinity[TestRepository.ItemCategoryEnum.Video.ToString()] = n;
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
          
            int n = JSONReponse.data.Count;

        

            user.Affinity[TestRepository.ItemCategoryEnum.Audio.ToString()] = n;
        }


        public static string[] FetchGiftCaseFriends(User user){

             string AnaLongTermToken = "CAAMewqUUav0BADBpQbA3mQZAwzZB1mmL2TzR7hrYildnnEHJUCipZC0QZAZAZCoKhwh6ZAHd80tCYSMIhluo6IeRBlkSctEK7ZAHHff7OnVPRe1hjTRW0FPsmbitIYtbCZC8Gj7bCfG39Lqv63ACaSs7TTSsd2p725c5LthCUwp4qA3pdZACWIqLDOfmKtcZCCHCrCIRuVknu2Ru4ZBuqAu1lajO";

             string requestUrl = "https://graph.facebook.com/" + user.Id + "/friends?access_token=" + AnaLongTermToken; //likes, music,games, movies, television, books
            // obtain the public profile data
            var request = WebRequest.CreateHttp(requestUrl);
            var stream = request.GetResponse().GetResponseStream();
            var reader = new StreamReader(stream);
            var result = reader.ReadToEnd();

            //profile = JsonConvert.DeserializeObject<FacebookPublicProfile>(result);
            dynamic JSONReponse = JsonConvert.DeserializeObject(result);
          
            int n = JSONReponse.data.Count;

            string [] temp = new string[2*n];

            for (int i = 0; i < n; i++)
            {
                temp[2*i] = JSONReponse.data[i].name.ToString(); //name i id može!!!
                temp[2*i + 1] = JSONReponse.data[i].id.ToString();
            }

            return temp;
           // user.Affinity[TestRepository.ItemCategoryEnum.Audio.ToString()] = n;
        }

        public static string[] FetchInviteableFriends(User user, int limit)  //WORKS ONLY FOR /ME or people i Have 
        {

            string AnaLongTermToken = "CAAMewqUUav0BADBpQbA3mQZAwzZB1mmL2TzR7hrYildnnEHJUCipZC0QZAZAZCoKhwh6ZAHd80tCYSMIhluo6IeRBlkSctEK7ZAHHff7OnVPRe1hjTRW0FPsmbitIYtbCZC8Gj7bCfG39Lqv63ACaSs7TTSsd2p725c5LthCUwp4qA3pdZACWIqLDOfmKtcZCCHCrCIRuVknu2Ru4ZBuqAu1lajO";

            string tempToken=null;

            if (user.ExtendedToken==null && user.ExtendedToken =="")
            {
                tempToken = DamirLongTermExtendedToken;
            }

            if (user.FacebookAccessToken == null && user.FacebookAccessToken == "")
            {
                tempToken = DamirLongTermExtendedToken;
            }

            if (tempToken == null)
            {
                tempToken = user.ExtendedToken;
            }
            
            //string requestUrl = "https://graph.facebook.com/" + "me" + "/invitable_friends?access_token=" + AnaLongTermToken; //likes, music,games, movies, television, books // NE MOŽE -> + user.Id + 
            //string requestUrl = "https://graph.facebook.com/" + user.Id + "/invitable_friends?access_token=" + DamirLongTermExtendedToken; //likes, music,games, movies, television, books // NE MOŽE -> + user.Id + 
            string requestUrl = "https://graph.facebook.com/" + "me" + "/invitable_friends?access_token=" + tempToken; 
            // obtain the public profile data
            var request = WebRequest.CreateHttp(requestUrl);
            var stream = request.GetResponse().GetResponseStream();
            var reader = new StreamReader(stream);
            var result = reader.ReadToEnd();

            //profile = JsonConvert.DeserializeObject<FacebookPublicProfile>(result);
            dynamic JSONReponse = JsonConvert.DeserializeObject(result);


            int n = JSONReponse.data.Count;
            if (limit < n)
            {
                n = limit;
            }

            string[] temp = new string[3 * n];

            for (int i = 0; i < n; i++)
            {
                temp[3 * i] = JSONReponse.data[i].name.ToString(); //name i id može!!! I picture i još par stvari za inviteable friends može!!!!
                temp[3 * i + 1] = JSONReponse.data[i].id.ToString();
                temp[3 * i + 2] = JSONReponse.data[i].picture.data.url.ToString();
            }

            return temp;
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