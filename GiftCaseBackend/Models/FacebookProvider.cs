using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using HtmlAgilityPack;
using Newtonsoft.Json;
using SimpleJSON;

namespace GiftCaseBackend.Models.Services
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