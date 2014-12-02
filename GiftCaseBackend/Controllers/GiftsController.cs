﻿using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Web.Http;
using GiftCaseBackend.Models;

namespace GiftCaseBackend.Controllers
{
    public class GiftsController : ApiController
    {
        /// <summary>
        /// Recommends some gifts
        /// URL example:
        /// http://giftcase.azurewebsites.net/api/Gifts/SuggestGift?userName=ana
        /// http://giftcase.azurewebsites.net/api/Gifts/SuggestGift?userName=ana&categoryId=1
        /// http://giftcase.azurewebsites.net/api/Gifts/SuggestGift?userName=ana&categoryName=Movie
        /// http://giftcase.azurewebsites.net/api/Gifts/SuggestGift?userName=ana&categoryName=Book&priceMin=0&priceMax=20
        /// </summary>
        /// <param name="username">Name of the friend to whom to recommend a gift for</param>
        /// <param name="count"></param>
        /// <param name="categoryId">Id of category of gift, if you want to search by id</param>
        /// <param name="categoryName">Name of the category if you want to search by category name</param>
        /// <param name="priceMin"></param>
        /// <param name="priceMax"></param>
        /// <returns>List of gift recommendations</returns>
        [HttpGet]
        public IEnumerable<Item> SuggestGift(string username, int count=3, int? categoryId=null, string categoryName = null,
            int priceMin=0, int priceMax = int.MaxValue)
        {
            //User tempuser = TestRepository.Users.Find(username); //take username and get a user, and then forward it to the Recommendation engine! ANA?

            User tempUser = new User { UserName = username };
            //GiftRecommendationEngine.CalculateAffinity(tempUser);
            tempUser.CalculateAffinity();

            if (categoryId == null)
            {
                categoryId = -1; //meaning all cats
            }
             IEnumerable<Item> gifts = GiftRecommendationEngine.RecommendGifts(tempUser, count, categoryId);

             /*
              //removed, it's in the gift recommendation engine now
             
             IEnumerable<Item> gifts = TestRepository.Items;

             // get category details
             ItemCategory category=null;
             if(categoryId!=null)
                 category = TestRepository.Categories.First(x => x.Id == categoryId);
             else if(categoryName!=null)
                 category = TestRepository.Categories.First(x => x.Name==categoryName);

             // if we are searching for games, search steam
             if (category != null && category.ParentCategory == 3)
             {
                 gifts = SteamProvider.ParseSteam(category.Id,count);
             }
             // if we are not searching for games, just return dummy results
             else
             {
                 if (categoryName != null)
                     gifts = gifts.Where(x => x.Category.Name == categoryName);
                 else if (categoryId != null)
                     gifts = gifts.Where(x => x.Category.Id == categoryId);
             }

              * */

             // filter by price
             //some free games have price = 0, we should ignore them?

            // priceMin = 2;
            // priceMax = 20;
            if (priceMin > 0 && priceMax < int.MaxValue && priceMin < priceMax) 
                gifts = gifts.Where(x => x.Price >= priceMin && x.Price <= priceMax);
            // return a certain count
            if (count > 0)
                gifts = gifts.Take(count);


             return gifts;
        }


        /// <summary>
        /// Gets the user's inbox - list of received gifts
        /// URL example:
        /// http://giftcase.azurewebsites.net/api/Gifts/Inbox?userId=Vlatko&count=3
        /// http://giftcase.azurewebsites.net/api/ana/Gifts/Inbox?count=3 <- not working
        /// </summary>
        /// <param name="userId">Id of the user using the app</param>
        /// <param name="count">Optional. Count of inbox items to get. If not specified returns all items.</param>
        /// <returns>Specified number of most recent received gifts.</returns>
        [HttpGet]
        [Route("api/Gifts/{userId}/Inbox")]
        [Route("api/Gifts/Inbox")]
        public IEnumerable<Gift> Inbox(string userId, int count=0)
        {
            var inbox = TestRepository.Gifts.Where(x => x.UserWhoReceivedTheGift.UserName == userId);
            if (count < 1)
                return inbox;
            return inbox.Take(count);
        }

        /// <summary>
        /// Gets the user's outbox
        /// URL example:
        /// http://giftcase.azurewebsites.net/api/Gifts/Outbox?userId=Vlatko&count=3
        /// http://giftcase.azurewebsites.net/api/ana/Gifts/Outbox?count=3  <- not working
        /// </summary>
        /// <param name="userId">Id of the user using the app</param>
        /// <param name="count">Optional. Count of outbox items to get. If not specified returns all of items.</param>
        /// <returns>Specified number of most recently given gifts.</returns>
        [HttpGet]
        [Route("api/Gifts/{userId}/Outbox")]
        [Route("api/Gifts/Outbox")]
        public IEnumerable<Gift> Outbox(string userId, int count = 0)
        {
            var outbox = TestRepository.Gifts.Where(x => x.UserWhoGaveTheGift.UserName == userId);
            if (count < 1)
                return outbox;
            return outbox.Take(count);
        }

        /// <summary>
        /// Gets the list of all available categories
        /// URL example:
        /// http://giftcase.azurewebsites.net/api/Gifts/CategoriesList
        /// </summary>
        /// <returns>List of categories</returns>
        [HttpGet]
        public List<ItemCategory> CategoriesList()
        {
            return TestRepository.Categories;
        }

        /// <summary>
        /// Updates the gift status
        /// URL example:
        /// http://giftcase.azurewebsites.net/api/Gifts/UpdateGiftStatus?giftId=1&userId=Ana&status=Claimed
        /// http://giftcase.azurewebsites.net/api/Gifts/1/UpdateGiftStatus?userId=Ana&status=Claimed
        /// </summary>
        /// <param name="giftId">Id of the gift</param>
        /// <param name="userId">Id of the user who received it</param>
        /// <param name="status">New status of the gift</param>
        /// <returns>True if successful, error message if not</returns>
        [HttpGet]
        [Route("api/Gifts/{giftId}/UpdateGiftStatus")]
        [Route("api/Gifts/UpdateGiftStatus")]
        public bool UpdateGiftStatus(int giftId, string userId, GiftStatus status)
        {
            return true;
        }

        /// <summary>
        /// Purchases and sends the gift to a user
        /// URL example:
        /// http://giftcase.azurewebsites.net/api/Gifts/SendGift?itemId=1&userId=Ana&contactUsername=Vlatko
        /// http://giftcase.azurewebsites.net/api/Gifts/1/SendGift?userId=Ana&contactUsername=Vlatko
        /// </summary>
        /// <param name="itemId">Id of the item that is being purchased</param>
        /// <param name="userId">Id of the user who purchased the gift</param>
        /// <param name="contactUsername">Username of the contact the user wants to buy the gift for</param>
        /// <returns>Gift details</returns>
        [HttpGet]
        [Route("api/Gifts/{itemId}/SendGift")]
        [Route("api/Gifts/SendGift")]
        public Gift SendGift(int itemId, string userId, string contactUsername)
        {
            var item = TestRepository.Items.Where(x => x.Id == itemId).FirstOrDefault();
            if(item==null)
                throw new Exception("No Item with that Id");

            return new Gift(){DateOfPurchase = DateTime.Now, 
                Item = item,
            Status = GiftStatus.NotReceivedYet,
            UserWhoGaveTheGift = new Contact(){UserName = userId, Status = UserStatus.Registered},
            UserWhoReceivedTheGift = new Contact(){UserName = contactUsername, Status = UserStatus.NonRegistered}};
        }

        /// <summary>
        /// ----------This doesn't work yet!------------------
        /// Downloads a gift that the current user has been given
        /// URL example:
        /// http://giftcase.azurewebsites.net/api/Gifts/DownloadGift?giftId=1&userId=Ana
        /// http://giftcase.azurewebsites.net/api/Gifts/1/DownloadGift?userId=Ana
        /// </summary>
        /// <param name="giftId">Id of the gift that needs to be downloaded</param>
        /// <param name="userId">Id of the user trying to download the gift</param>
        /// <returns>Gift binary data stream</returns>
        [HttpGet]
        [Route("api/Gifts/{giftId}/DownloadGift")]
        [Route("api/Gifts/DownloadGift")]
        public StreamContent DownloadGift(int giftId, string userId)
        {
            var content = new FileStream("https://lh5.googleusercontent.com/-z4GINoMoCgA/AAAAAAAAAAI/AAAAAAAAABQ/CM0fRlsGcD8/photo.jpg", FileMode.Open, FileAccess.Read);
            var stream = new StreamContent(content);
            return stream;
        }
    }
}
