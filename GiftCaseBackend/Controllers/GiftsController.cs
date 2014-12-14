using System;
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
        #region SuggestGift
        /// <summary>
        /// Recommends some gifts
        /// todo: use gift recommendation engine to do recommendations
        /// todo: integrate amazon
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
        public IEnumerable<Item> SuggestGift(string userName, int count=3, int? categoryId=null, string categoryName = null,
            float priceMin=0, float priceMax = 100000)
        {
            //User tempuser = TestRepository.Users.Find(username); //take username and get a user, and then forward it to the Recommendation engine! ANA?

            //problem oko user ili contact
            Contact tempContact = TestRepository.Friends.Where(s=> s.UserName == userName).ToList()[0];
            User tempUser = new User { UserName = userName, /* Id = "10152464438050382" */ Id =tempContact.Id };

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
        #endregion

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
        public IEnumerable<Gift> Inbox(string userId, int count = 0)
        {
            //userId = UserController.CheckAutherization();
              
            IEnumerable<Gift> inbox;
            try
            {
                // test if the user exists, if he exists get inbox from the BaaS
                if (BaaS.DoesUserDataExist(userId))
                    inbox = BaaS.GetInbox(userId);
                // if he doesn't exist, fallback to test results
                else
                    inbox = TestRepository.Gifts.Where(x => x.UserWhoReceivedTheGift.Id == userId);

            }
            catch (Exception e)
            {
                // if connection to BaaS fails, fallback to test results
                inbox = TestRepository.Gifts.Where(x => x.UserWhoReceivedTheGift.Id == userId);
            }
            
            if (count >= 1)
                inbox = inbox.Take(count);
            return inbox;
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
            IEnumerable<Gift> outbox;
            try
            {
                if (BaaS.DoesUserDataExist(userId))
                    outbox = BaaS.GetOutbox(userId);
                // fallback to test results
                else
                    outbox = TestRepository.Gifts.Where(x => x.UserWhoGaveTheGift.Id == userId);
            }
            catch (Exception e)
            {
                outbox = TestRepository.Gifts.Where(x => x.UserWhoGaveTheGift.Id == userId);
            }
            

            if (count < 1)
                return outbox;
            return outbox.Take(count);
        }

        /// <summary>
        /// Updates the gift status
        /// todo: actually make it work
        /// todo: send notifications to users
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
            // try to get the gift
            // if gift doesn't exist, throw an exception: gift doesn't exist
            // when you get the gift check if this user has the permission to update status
            // if the user is the receiving user update the gift status

            return true;
        }

        /// <summary>
        /// Purchases and sends the gift to a user
        /// URL example:
        /// http://giftcase.azurewebsites.net/api/Gifts/SendGift?itemId=1&store=Steam&userId=Ana&contactId=Vlatko
        /// http://giftcase.azurewebsites.net/api/Gifts/1/SendGift?userId=Ana&store=Steam&contactId=Vlatko
        /// </summary>
        /// <param name="itemId">Id of the item that is being purchased</param>
        /// <param name="store">Name of the store the item is being purchased from.
        /// <param name="userId">Id of the user who purchased the gift</param>
        /// <param name="contactId">Username of the contact the user wants to buy the gift for</param>
        /// <returns>Gift details</returns>
        [HttpGet]
        [Route("api/Gifts/{itemId}/SendGift")]
        [Route("api/Gifts/SendGift")]
        public Gift SendGift(string itemId,
            Store store, //string storeName, 
            string userId, string contactId)
        {/*
            Store store = Store.Amazon;
            try { store = (Store)Enum.Parse(typeof(Store), storeName); }
            catch (Exception) { }
                */

            Gift gift = new Gift()
            {
                DateOfPurchase = DateTime.Now,
                Status = GiftStatus.NotReceivedYet
            };

            var item = new Item();
            item.Id = itemId;
            item.Store = store;
            gift.Item = item;

            //todo: try to get the item from the BaaS
            
            //todo: if item doesn't exist get it from content providers

            //todo: if item could not be found fallback to test repository
            /*var item = TestRepository.Items.Where(x => x.Id == itemId).FirstOrDefault();
            if(item==null)
                throw new Exception("No Item with that Id");*/
            
            // when you get the item, create a gift
            //gift.Item = item;


            //todo: get user details about current user and receiving user and fill up the related gift fields
            gift.UserWhoGaveTheGift = new Contact() {Id = userId, Status = UserStatus.Registered};
            gift.UserWhoReceivedTheGift = new Contact(){Id = contactId, Status = UserStatus.Registered};

            //todo: add the gift into the database
            BaaS.AddNewGift(gift);

            //todo: send notifications to users

            return gift;
        }

        #region Finished
        /// <summary>
        /// Gets the list of all available categories
        /// URL example:
        /// http://giftcase.azurewebsites.net/api/Gifts/CategoriesList
        /// </summary>
        /// <returns>List of categories</returns>
        [HttpGet]
        public List<ItemCategory> CategoriesList()
        {
            return TestRepository.Categories.Values.ToList();
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
        #endregion

        [HttpGet]
        public IEnumerable<Item> Test()
        {
            return AmazonProvider.FindRelatedItems(new[] { "B00003CXB4" });

        }
    }
}
