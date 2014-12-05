using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Net;

namespace GiftCaseBackend.Models.ContentProviders
{
    public static class TelcoDataProvider
    {
        public static TelcoData getTelcoData(string userId, string telcoURL = "http://limitless-brushlands-2961.herokuapp.com/") 
        {
            var wc = new WebClient();
            var jsonStr = wc.DownloadString(telcoURL + "Profile/" + userId);

            JToken token = JObject.Parse(jsonStr);

            string location = (string)token.SelectToken("location");
            string plan = (string)token.SelectToken("billingplan");

            var data = new TelcoData();

            data.Location = location;
            data.UserId = userId;
            data.BillingPlan = plan;

            return data;
        }
    }
}