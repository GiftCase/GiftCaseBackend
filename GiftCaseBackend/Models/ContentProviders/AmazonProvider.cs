using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;
using Amazon.PAAPI.WCF;
using GiftCaseBackend.Amazon;

namespace GiftCaseBackend.Models
{
    public static class AmazonProvider
    {
        private const string AccessKeyId = "AKIAIU6U5OAEQZKTPKRQ";
        private const string SecretKey = "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";
        private const string EndPoint = // "http://webservices.amazon.com/onca/soap?Service=AWSECommerceService" +
                                        "https://ecs.amazonaws.com/onca/soap?Service=AWSECommerceService";

        private static AWSECommerceServicePortTypeClient Client;

        public static void Test()
        {
            // create a WCF Amazon ECS client
            AWSECommerceServicePortTypeClient client = new AWSECommerceServicePortTypeClient();
            /*BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.MaxReceivedMessageSize = int.MaxValue;
            AWSECommerceServicePortTypeClient client = new AWSECommerceServicePortTypeClient(
                binding,
                new EndpointAddress("https://webservices.amazon.com/onca/soap?Service=AWSECommerceService"));
            */
            // add authentication to the ECS client
            client.ChannelFactory.Endpoint.Behaviors.Add(new AmazonSigningEndpointBehavior(AccessKeyId, SecretKey));
 
            // prepare an ItemSearch request
            ItemSearchRequest request = new ItemSearchRequest();
            request.SearchIndex = "Books";
            request.Title = "WCF";
            request.ResponseGroup = new string[] { "Small" };

            ItemSearch itemSearch = new ItemSearch();
            itemSearch.Request = new ItemSearchRequest[] { request };
            itemSearch.AWSAccessKeyId = AccessKeyId;
            itemSearch.AssociateTag = "";

            // issue the ItemSearch request
            ItemSearchResponse response = client.ItemSearch(itemSearch);

            /*
            Client = new Amazon.AWSECommerceServicePortTypeClient(EndPoint);
               // new BasicHttpBinding(BasicHttpSecurityMode.Transport),new EndpointAddress(EndPoint));
            var response = Client.ItemLookup(new ItemLookup(){AWSAccessKeyId = AccessKeyId, //AssociateTag = "XYZ", 
                //MarketplaceDomain = "", 
                Request = new []{new ItemLookupRequest(){}}});*/

        }
        /*
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            // prepare the data to sign
            DateTime now = DateTime.UtcNow;
            string timestamp = now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            string signMe = operation + timestamp;
            byte[] bytesToSign = Encoding.UTF8.GetBytes(signMe);

            // sign the data
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(SecretKey);
            HMAC hmacSha256 = new HMACSHA256(secretKeyBytes);
            byte[] hashBytes = hmacSha256.ComputeHash(bytesToSign);
            string signature = Convert.ToBase64String(hashBytes);

            // add the signature information to the request headers
            request.Headers.Add(new AmazonHeader("AWSAccessKeyId", AccessKeyId));
            request.Headers.Add(new AmazonHeader("Timestamp", timestamp));
            request.Headers.Add(new AmazonHeader("Signature", signature));

            return null;
        }*/
    }
}