using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.pushNotification;
using com.shephertz.app42.paas.sdk.csharp.social;
using com.shephertz.app42.paas.sdk.csharp.storage;
using com.shephertz.app42.paas.sdk.csharp.user;
using Newtonsoft.Json;

namespace GiftCaseBackend.Models
{
    public static class BaaS
    {
        private const string APIkey = "7fc41ebb0ac5c4c9342bc47ac49e1bc097eb67de7e68828bbdf009d1e5cc26fb";
        private const string SecretKey = "3fd5ebed57cbe849f0915cbe394ad7ebfd15082c24345cd18cb630bcfab02ab5";
        private const string DatabaseName = "USERDATA";
        private const string UserCollection = "Users";
        private const string GiftCollection = "Gifts";
        private const string AppDataCollection = "AppData";

        public static ServiceAPI Api { get; private set; }
        public static UserService UserService { get; private set; }
        public static StorageService StorageService { get; private set; }

        public static SocialService SocialService { get; set; }
        public static PushNotificationService PushNotificationService { get; set; }

        public static void Initialize()
        {
            Api = new ServiceAPI(APIkey, SecretKey);
            //Build User Service  
            UserService = Api.BuildUserService();
            // Using userService reference, you should be able to call all its method like create user/update user/authenticate etc.  
            //Build Storage Service  
            StorageService = Api.BuildStorageService();
            //Build Social Service
            SocialService = Api.BuildSocialService();
            //Build Push Notification Service
            PushNotificationService = Api.BuildPushNotificationService();   
        }

        public static string GetKeys(string service)
        {
            try
            {
                var document = StorageService.FindDocumentByKeyValue(DatabaseName, AppDataCollection, "Service", service);
                return document.GetJsonDocList()[0].jsonDoc;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        /// <summary>
        /// Saves data about a new GiftCase user into the database. You have to check if that user already exists
        /// before calling this to avoid creating duplicate entries for the same user.
        /// </summary>
        /// <param name="user">User data that needs to be saved into the database</param>
        public static void CreateUser(User user)
        {
            var serializedData = JsonConvert.SerializeObject(user.Shorten());
            StorageService.InsertJSONDocument(DatabaseName, UserCollection, serializedData);
        }

        /// <summary>
        /// When a new user registers in the GiftCase app we have to add his data in the database.
        /// However, there is a possibility that he is a friend of a GiftCase user so his data would already be in the database.
        /// If his data is already in the database call this method, it updates user data to include fields that
        /// only registered users have.
        /// </summary>
        /// <param name="user">User whose data needs to be saved into the database</param>
        public static void UpdateNonregisteredToRegisteredUserIfNeeded(User user)
        {
            try
            {
                var document = StorageService.FindDocumentByKeyValue(DatabaseName, UserCollection, "Id", user.Id);
                var contact = JsonConvert.DeserializeObject<Contact>(document.GetJsonDocList()[0].jsonDoc);
                if (contact.Status == UserStatus.NonRegistered)
                {
                    var serializedData = JsonConvert.SerializeObject(user.Shorten());
                    StorageService.InsertJSONDocument(DatabaseName, UserCollection, serializedData);
                    // change user data
                    StorageService.UpdateDocumentByDocId(DatabaseName, UserCollection,
                        document.GetJsonDocList()[0].docId, serializedData);
                }
            }
            catch (Exception) { }
        }

        public static void UpdateUserData(User user)
        {
            try
            {
                var document = StorageService.FindDocumentByKeyValue(DatabaseName, UserCollection, "Id", user.Id);
                var serializedData = JsonConvert.SerializeObject(user.Shorten());
                StorageService.UpdateDocumentByDocId(DatabaseName, UserCollection,
                                     document.GetJsonDocList()[0].docId, serializedData);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// When we collect data about friends of a GiftCase user, we save that data into the database if it already isn't
        /// in the database. Call this method if you come across a non registered user whose data is still not in the database.
        /// </summary>
        /// <param name="facebookId">Facebook id of the non registered user</param>
        public static void CreateNonregisteredUser(string facebookId, string username=null, string imageUrl=null)
        {
            var user = new Contact(); // maybe we should put new User() here instead of contact
            user.Id = facebookId;
            user.Status = UserStatus.NonRegistered;
            user.UserName = username;
            user.ImageUrl = imageUrl;

            // try to fetch additional facebook data
            try
            {
                if(username==null || imageUrl==null)
                {
                    var profile = SocialService.GetFacebookProfilesFromIds(new[] { facebookId }).GetPublicProfile()[0];
                    if (imageUrl == null)
                        user.ImageUrl = profile.GetPicture();
                    if (username == null)
                        user.UserName = profile.GetName();
                }
            }
            catch (Exception e) 
            {

            }
            if (user.UserName == null)
                user.UserName = "";
            user.Name = user.UserName;

            if (user.ImageUrl == null)
                user.ImageUrl = "";

            // we are checking outside this function for performance reasons
           // if (DoesUserDataExist(facebookId) == false)
            {
                var serializedData = JsonConvert.SerializeObject(user);
                StorageService.InsertJSONDocument(DatabaseName, UserCollection, serializedData);
            }
        }

        /// <summary>
        /// This method checks if user already exists in the database, regardless if he is a registered or non registered user.
        /// </summary>
        /// <param name="facebookId">Facebook id of the user</param>
        /// <returns>True if user data exists, false otherwise</returns>
        public static bool DoesUserDataExist(string facebookId)
        {
            try
            {
                var document = StorageService.FindDocumentByKeyValue(DatabaseName, UserCollection, "Id", facebookId);
                if (document == null || document.GetJsonDocList().Count <= 0)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
            return true;

        }

        /// <summary>
        /// Serializes gift data into the database
        /// </summary> 
        /// <param name="gift">Gift to serialize</param>
        public static void AddNewGift(Gift gift)
        {
            try
            {
            var serializedData = JsonConvert.SerializeObject(gift.Shorten());
            StorageService.InsertJSONDocument(DatabaseName, GiftCollection, serializedData);
            }
            catch (Exception e) { }
        }

        public static Gift GetGift(string giftId)
        {
            try
            {
                var document = StorageService.FindDocumentByKeyValue(DatabaseName, GiftCollection, "GiftId", giftId);
                var shortGift = JsonConvert.DeserializeObject<ShortGift>(document.GetJsonDocList()[0].jsonDoc);
                if (shortGift == null)
                    return null;
                return shortGift.ToGift();
            }
            catch (Exception e) { return null; }
        }

        public static void UpdateGiftStatus(string giftId, GiftStatus status)
        {
            try
            {
                var document = StorageService.FindDocumentByKeyValue(DatabaseName, GiftCollection, "GiftId", giftId);
                var shortGift = JsonConvert.DeserializeObject<ShortGift>(document.GetJsonDocList()[0].jsonDoc);
                if (shortGift.Status != status)
                {
                    shortGift.Status = status;
                    var serializedData = JsonConvert.SerializeObject(shortGift);
                    StorageService.UpdateDocumentByDocId(DatabaseName, GiftCollection,
                        document.GetJsonDocList()[0].docId, serializedData);
                }
            }
            catch (Exception) { }
        }

        public static bool LogOut(string userId, string deviceId)
        {
            try
            {
                PushNotificationService.UnSubscribeDevice(userId, deviceId);
                //PushNotificationService.DeleteDeviceToken(userId, deviceId);
                return true;
            }
            catch (Exception e) { }
            return false;
        }

        public static User GetUser(string userId)
        {
            try
            {
                var document = StorageService.FindDocumentByKeyValue(DatabaseName, UserCollection, "Id", userId);
                var contact = JsonConvert.DeserializeObject<User>(document.GetJsonDocList()[0].jsonDoc);
                contact.UserName = contact.Name;
                return contact;
            }
            catch (Exception e)
            { return null; }
        }

        /// <summary>
        /// todo: try if this works
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Contact GetContact(string userId)
        {
            try
            {
                var document = StorageService.FindDocumentByKeyValue(DatabaseName, UserCollection, "Id", userId);
                var contact = JsonConvert.DeserializeObject<Contact>(document.GetJsonDocList()[0].jsonDoc);
                return contact;
            }
            catch (Exception e)
            { return null; }
        }

        /// <summary>
        /// Todo: sort? get only $count items?
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static IEnumerable<Gift> GetInbox(string userId)
        {
            //Query q1 = QueryBuilder.Build("UserWhoReceivedTheGift", userId, Operator.EQUALS); 
            
            var documents = StorageService
                //.FindDocumentsByQuery(DatabaseName, GiftCollection, new Query("")).GetJsonDocList();
                .FindDocumentByKeyValue(DatabaseName, GiftCollection, "IdOfUserWhoReceivedTheGift", userId).GetJsonDocList();
            var inbox = new List<Gift>(documents.Count);
            foreach (var jsonDocument in documents)
            {
                var gift = JsonConvert.DeserializeObject<ShortGift>(jsonDocument.jsonDoc);
                inbox.Add(gift.ToGift());
            }
            return inbox;
        }

        public static List<Gift> GetOutbox(string userId)
        {
            //Query q1 = QueryBuilder.Build("UserWhoReceivedTheGift", userId, Operator.EQUALS); 

            var documents = StorageService
                //.FindDocumentsByQuery(DatabaseName, GiftCollection, new Query("")).GetJsonDocList();
                .FindDocumentByKeyValue(DatabaseName, GiftCollection, "IdOfUserWhoGaveTheGift", userId).GetJsonDocList();
            List<Gift> inbox = new List<Gift>(documents.Count);
            foreach (var jsonDocument in documents)
            {
                var gift = JsonConvert.DeserializeObject<ShortGift>(jsonDocument.jsonDoc);
                inbox.Add(gift.ToGift());
            }
            return inbox;
        }
    }
}