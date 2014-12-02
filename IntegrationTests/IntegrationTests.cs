using System;
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
