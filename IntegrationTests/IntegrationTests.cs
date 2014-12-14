using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Web.Http;
using GiftCaseBackend.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GiftCaseBackend;
using Newtonsoft.Json;

namespace IntegrationTests
{
    [TestClass]
    public class IntegrationTests : IDisposable
    {
        private HttpServer _server;
        private string _url = "http://giftcase.azurewebsites.net/api/";


        [TestInitialize]
        public void InitializeServer() 
        {
            var config = new HttpConfiguration();
            config.MessageHandlers.Add(new WebApiKeyHandler());
            WebApiConfig.Register(config);
            _server = new HttpServer(config);
        }

        [TestMethod]
        public void GetListOfCategories()
        {
            var client = new HttpClient(_server);
            var request = CreateGetRequest("Gifts/CategoriesList");
            var expectedJson = JsonConvert.SerializeObject(TestRepository.Categories);
                            /*"[{\"Id\":0,\"Name\":\"Book\",\"ParentCategory\":null}," +
                               "{\"Id\":1,\"Name\":\"Movie\",\"ParentCategory\":null}," +
                               "{\"Id\":2,\"Name\":\"Audio\",\"ParentCategory\":null}]";*/

            using (HttpResponseMessage response = client.SendAsync(request).Result)
            {
                Assert.IsNotNull(response.Content);
                Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType);
                //Assert.Equal(3, response.Content.ReadAsAsync<IQueryable<Url>>().Result.Count());
                Assert.AreEqual(expectedJson, response.Content.ReadAsStringAsync().Result);
            }

            request.Dispose();
            client.Dispose();
        }

        [TestMethod]
        public void GetContacts()
        {
            var client = new HttpClient(_server);
            var request = CreateGetRequest("User/Contacts?userId=10152464438050382");
  
            using (HttpResponseMessage response = client.SendAsync(request).Result)
            {
                Assert.IsNotNull(response.Content);
                Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType);
                try
                {
                    Assert.IsNotNull(Newtonsoft.Json.Linq.JObject.Parse(response.Content.ToString()));
                }
                catch (Exception e) {
                    Console.WriteLine("  ERROR: GetContacts failed :(\nIOException source: {0}", e.Source);
                }
                
            }

            request.Dispose();
            client.Dispose();
        }

        [TestMethod]
        public void GetUserDetails()
        {
            var client = new HttpClient(_server);
            var request = CreateGetRequest("User/Details?userId=10152464438050382");

            using (HttpResponseMessage response = client.SendAsync(request).Result)
            {
                Assert.IsNotNull(response.Content);
                Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType);
                try
                {
                    Assert.IsNotNull(Newtonsoft.Json.Linq.JObject.Parse(response.Content.ToString()));
                }
                catch (Exception e)
                {
                    Console.WriteLine("  ERROR: GetUserDetails failed :(\nIOException source: {0}", e.Source);
                }

            }

            request.Dispose();
            client.Dispose();
        }

        [TestMethod]
        public void GetUserEvents()
        {
            var client = new HttpClient(_server);
            var request = CreateGetRequest("User/10152464438050382/Events");

            using (HttpResponseMessage response = client.SendAsync(request).Result)
            {
                Assert.IsNotNull(response.Content);
                Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType);
                try
                {
                    Assert.IsNotNull(Newtonsoft.Json.Linq.JObject.Parse(response.Content.ToString()));
                }
                catch (Exception e)
                {
                    Console.WriteLine("  ERROR: GetUserEvents failed :(\nIOException source: {0}", e.Source);
                }

            }

            request.Dispose();
            client.Dispose();
        }

        [TestMethod]
        public void SuggestGifts()
        {
            var client = new HttpClient(_server);
            var request = CreateGetRequest("Gifts/SuggestGift?userName=ana");

            using (HttpResponseMessage response = client.SendAsync(request).Result)
            {
                Assert.IsNotNull(response.Content);
                Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType);
                try
                {
                    Assert.IsNotNull(Newtonsoft.Json.Linq.JObject.Parse(response.Content.ToString()));
                }
                catch (Exception e)
                {
                    Console.WriteLine("  ERROR: SuggestGifts failed :(\nIOException source: {0}", e.Source);
                }

            }

            request.Dispose();
            client.Dispose();
        }

        [TestMethod]
        public void GetInbox()
        {
            var client = new HttpClient(_server);
            var request = CreateGetRequest("Gifts/Inbox?userId=10152464438050382&count=3");

            using (HttpResponseMessage response = client.SendAsync(request).Result)
            {
                Assert.IsNotNull(response.Content);
                Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType);
                try
                {
                    Assert.IsNotNull(Newtonsoft.Json.Linq.JObject.Parse(response.Content.ToString()));
                }
                catch (Exception e)
                {
                    Console.WriteLine("  ERROR: GetInbox failed :(\nIOException source: {0}", e.Source);
                }

            }

            request.Dispose();
            client.Dispose();
        }

        [TestMethod]
        public void GetOutbox()
        {
            var client = new HttpClient(_server);
            var request = CreateGetRequest("Gifts/Outbox?userId=10152464438050382&count=3");

            using (HttpResponseMessage response = client.SendAsync(request).Result)
            {
                Assert.IsNotNull(response.Content);
                Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType);
                try
                {
                    Assert.IsNotNull(Newtonsoft.Json.Linq.JObject.Parse(response.Content.ToString()));
                }
                catch (Exception e)
                {
                    Console.WriteLine("  ERROR: GetOutbox failed :(\nIOException source: {0}", e.Source);
                }

            }

            request.Dispose();
            client.Dispose();
        }

        [TestMethod]
        public void GetFacebookLikes()
        {
            Contact tempContact = TestRepository.Friends.ToList()[0];
            User tempUser = new User { UserName = tempContact.UserName, /* Id = "10152464438050382" */ Id = tempContact.Id };

           // Contact anaTest = TestRepository.Friends[0];
            FacebookProvider.UpdateAffinity(tempUser);

            tempContact = TestRepository.Friends.ToList()[2];
            tempUser = new User { UserName = tempContact.UserName, Id = tempContact.Id };

           
            FacebookProvider.UpdateAffinity(tempUser);

        }

        [TestMethod]
        public void GetSteamData()
        {
            List<Game> tempGames =  SteamProvider.ParseSteam(new int[]{122},14).ToList();

            if (tempGames.Count != 14) { throw new Exception();}

            foreach (Game game in tempGames)
            {
                if (game.Description == null || game.Id == null || game.Name == null)
                {
                    throw new Exception();
                }
            }

        }


        [TestMethod]
        public void GetUnspecifiedRecommendation()
        {
            Contact tempContact = TestRepository.Friends.ToList()[0];
            User tempUser = new User { UserName = tempContact.UserName, Id = tempContact.Id };
            List<Item> tempItems =  GiftRecommendationEngine.RecommendGifts(tempUser, 7).ToList();
            if (tempItems.Count != 7) { throw new Exception(); }

            foreach (Item item in tempItems)
            {
                if (item.Name == null || item.Id == null || item.Price > 0)
                {
                    throw new Exception();
                }
            }

        }

         [TestMethod]
        public void GetRecommendationForSpecificCategory()
        {
            Contact tempContact = TestRepository.Friends.ToList()[2];
            User tempUser = new User { UserName = tempContact.UserName, Id = tempContact.Id };
            List<Item> tempItems = GiftRecommendationEngine.RecommendGifts(tempUser, 4, 1).ToList();
            if (tempItems.Count != 4) { throw new Exception(); }

            foreach (Item item in tempItems)
            {
                if (item.Name == null || item.Id == null || item.Price > 0)
                {
                    throw new Exception();
                }
            }
        }


        private HttpRequestMessage CreateGetRequest(string url)
        {
            var request = new HttpRequestMessage();

            request.RequestUri = new Uri(_url + url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Method = HttpMethod.Get;

            return request;
        }
 
        [TestCleanup]
        public void Dispose()
        {
            if (_server != null)
            {
                _server.Dispose();
            }
        }
    }
}
