using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;
using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.pushNotification;
using com.shephertz.app42.paas.sdk.csharp.social;
using GiftCaseBackend.Models;
using Newtonsoft.Json;

namespace GiftCaseBackend.Controllers
{
    public class UserController : ApiController
    {
        /*
         * Instructions:
         * How you can access the REST api provided by this controller:
         * use the url:  /api/User/NameOfTheAction?parameterName=value&parameter2=value
         * 
         * Name of the action is the method name. If you want to change the url into something different
         * than the name of the action put an attribute: [ActionName("NewName")] above the method
         * 
         * The results returned by REST api are either in JSON or XML format. The format depends on what the requester
         * specified he would like to receive. If not specified it defaults to JSON.
         * 
         * The server will automatically try to map what methods should be called with what type of request. If you
         * want to specify it manually you can put attributes above the method, eg: [HttpGet],[HttpPost]
         */



        #region Login
        /// <summary>
        /// Links facebook account with BaaS. Creates user profiles in the database, upgrades profiles from non registered to registered if they exist.
        /// Todo: update the profile data if it has changed
        /// Sets up push notification services.
        /// --------------------------------------
        /// URL example:
        /// http://giftcase.azurewebsites.net/api/User/Login?userName=ana&accessToken=someGarbage&deviceToken=whatever
        /// </summary>
        /// <param name="userId">User's facebook user id</param>
        /// <param name="accessToken">Facebook access token</param>
        /// <param name="deviceToken">A token identifying the device user is logging in from</param>
        /// <returns>status message</returns>
        [HttpGet]
        public User Login(string userId, string accessToken, string deviceToken)
        {
            User user = null;
            try
            {
                var social = BaaS.SocialService.LinkUserFacebookAccount(userId, accessToken);

                user =  new User()
                {
                    Id = userId,
                    FacebookAccessToken = accessToken,
                    //Gender = social.GetFacebookProfile().,
                    //Friends = Contacts(userId).ToList(),
                    Status = UserStatus.Registered,
                    
                };

                FacebookProvider.UpdateUserInfoWithPublicProfile(user);

                // creates user details data entry in the database
                if (BaaS.DoesUserDataExist(userId))
                    BaaS.UpdateNonregisteredToRegisteredUserIfNeeded(user);
                else
                    BaaS.CreateUser(user);
            }
            catch (Exception ex)
            {
                if (user == null)
                {
                    //SocialExceptionHandling(ex);
                    user = new User()
                    {
                        Id = userId,
                        UserName = "NoName",
                        FacebookAccessToken = accessToken,
                        //Friends = Contacts(userId).ToList(),
                        Status = UserStatus.Registered,
                    };
                }
            }
            /*
            try { var push = BaaS.PushNotificationService.StoreDeviceToken(userId, deviceToken, DeviceType.ANDROID); }
            catch(Exception e){}
            */

            // remember current user so we can authenticate the requests
            HttpContext.Current.Session["user"] = user;

            return user;
        }
        #endregion

        #region Contacts
        /// <summary>
        /// Gets a list of all user friends. Currently only gets friends who are registered GiftCase users.
        /// Todo: find a way to list all friends, not just registered friends.
        /// --------------------------------------
        /// URL example:
        /// http://giftcase.azurewebsites.net/api/User/Contacts?userId=ana
        /// http://giftcase.azurewebsites.net/api/User/ana/Contacts
        /// </summary>
        /// <param name="userId">id of the user</param>
        /// <returns>list of friends as JSON or XML</returns>
        [HttpGet]
        [Route("api/User/{userId}/Contacts")]
        [Route("api/User/Contacts")]
        public IEnumerable<Contact> Contacts(string userId=null)
        {
            //userId = CheckAutherization();

            IEnumerable<Contact> friends = null;
            try
            {
                // try to get user's registered friends
                var social =BaaS.SocialService.GetFacebookFriendsFromLinkUser(userId);
                friends = social.GetFriendList().Select(x=>
                    new Contact()
                    {
                        Id = x.id,
                        UserName = x.name,
                        ImageUrl = x.GetPicture(),
                    });

                // if we were able to get non registered friends, add them to the database
                foreach (var contact in friends)
                {
                    contact.Status = (BaaS.DoesUserDataExist(contact.Id)) ? UserStatus.NonRegistered : UserStatus.Registered;
                    if(contact.Status==UserStatus.NonRegistered)
                        BaaS.CreateNonregisteredUser(contact.Id);
                }

                return friends;
            }
            catch(Exception ex)
            {
                //fallback for testing purposes
                if (friends != null)
                    return friends;
                // for testing purposes
                return TestRepository.Friends;
            }
        }
        #endregion

        #region ContactsV2_Damir
        [HttpGet]
        [Route("api/User/{userId}/ContactsV2")]
        [Route("api/User/ContactsV2")]
        public IEnumerable<User>  ContactsV2(string userId=null)
        {
            //userId = CheckAutherization();

            User tempUser = new User{ Id = userId};
            if (userId == null)
            {
                tempUser.Id = "me";
            }

             //ako je null onda uzmi me
            string [] tempResult = FacebookProvider.FetchGiftCaseFriends(tempUser);
            
            List<User> korisnici = new List<User> ();
            

           for (int i = 0; i < tempResult.Length; i+=2)
			{
                korisnici.Add(new User { UserName = tempResult[i], Id = tempResult[i+1] });
			}

            return korisnici;
        }
        #endregion

        #region InvitesV2_Damir
        [HttpGet]
        //[Route("api/User/{userId}/ContactsV2")] //only works as /me/ !!!!!!
        [Route("api/User/InvitesV2")]
        public IEnumerable<User> InvitesV2(string userId = null)
        {
            //userId = CheckAutherization();
            int limit = 10; //too many inviteable friends makes the app go 2 slow!!!!

            User tempUser = new User { Id = userId };
            if (userId == null)
            {
                tempUser.Id = "me";
            }

            //ako je null onda uzmi me


            string[] tempResult = FacebookProvider.FetchInviteableFriends(tempUser,limit);

            List<User> korisnici = new List<User>();

            /* //krivo, mislim!
            if (limit < tempResult.Length)
            {
                limit = tempResult.Length;
            }
             */ 

            for (int i = 0; i < limit; i += 3)
            {
                korisnici.Add(new User { UserName = tempResult[i], Id = tempResult[i + 1], ImageUrl= tempResult[i+2]});
            }

            return korisnici.Take(5); //takes only 5, cuz otherwise it loads for too long, if you have a 100 friends
        }
        #endregion


        #region Low priority
        /// <summary>
        /// Currently doesn't do anything.
        /// --------------------------------------
        /// Url example:
        /// http://giftcase.azurewebsites.net/api/User/Logout?userId=ana&deviceToken=whatever
        /// </summary>
        /// <param name="userId">Id of the logged in user</param>
        /// <param name="deviceToken">Token of the device that needs to be logged out</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/User/{userId}/LogOut")]
        [Route("api/User/LogOut")]
        public bool LogOut(string userId, string deviceToken)
        {
            //remove registered device id's so the user doesn't get notifications
            var result = BaaS.LogOut(userId, deviceToken);
            if(result)
                HttpContext.Current.Session["user"] = null;
            return result;
        }


        /// <summary>
        /// Todo: we might not be able to do this :(
        /// Sends an invitation to a non giftcase user
        /// Url example:
        /// http://giftcase.azurewebsites.net/api/User/SendInvitation?userId=ana&email=bla@bla.vom&userName=Vlatko&text=blabla
        /// http://giftcase.azurewebsites.net/api/User/userId/SendInvitation?email=bla@bla.vom&userName=Vlatko&text=blabla
        /// </summary>
        /// <param name="userId">Id of the logged in giftcase user sending the invitation</param>
        /// <param name="email">Email where invitation will be sent</param>
        /// <param name="userName">Username of the user being invited</param>
        /// <param name="text">Optional parameter. Customized text that will be included in the email sent to the user being invited.</param>
        /// <returns>True if successful, otherwise error message</returns>
        [HttpGet]
        [Route("api/User/{userId}/SendInvitation")]
        [Route("api/User/SendInvitation")]
        public bool SendInvitation(string userId, string email, string userName, string text="")
        {
            return true;
        }

        /// <summary>
        /// Gets details about the logged in user. Used if the client app needs to refresh user details.
        /// Url example:
        /// http://giftcase.azurewebsites.net/api/User/Details?userId=ana
        /// http://giftcase.azurewebsites.net/api/User/userId/Details
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <returns>User details</returns>
        [HttpGet]
        [Route("api/User/{userId}/Details")]
        [Route("api/User/Details")]
        public User Details(string userId=null)
        {
            //userId = CheckAutherization();

            return (User)HttpContext.Current.Session["user"];
            /*
            User user = null;
            try
            {
                if(!BaaS.DoesUserDataExist(userId))
                user = new User()
                {
                    Id = userId,
                    UserName = "ana",
                    FacebookAccessToken = "gsjvnker",
                    //Friends = Contacts("ana").ToList(),
                    Status = UserStatus.Registered,
                };
                else
                    user = BaaS.GetUser(userId);
            }
            catch (Exception)
            {
                user = new User()
                {
                    Id = userId,
                    UserName = "ana",
                    FacebookAccessToken = "gsjvnker",
                    //Friends = Contacts("ana").ToList(),
                    Status = UserStatus.Registered,
                };
            }
            
            return user;*/
        }

        #endregion

        /// <summary>
        /// Gets the list of upcoming events
        /// Url example:
        /// http://giftcase.azurewebsites.net/api/User/Events?userId=ana
        /// http://giftcase.azurewebsites.net/api/User/userId/Events
        /// </summary>
        /// <param name="userId">Id of the current user</param>
        /// <returns>List of upcoming events</returns>
        [HttpGet]
        [Route("api/User/{userId}/Events")]
        [Route("api/User/Events")]
        public IEnumerable<GiftcaseEvent> Events(string userId)
        {
            return TestRepository.Events;
            //.Where(x=>x.RelatedContacts.Count!=1 && x.RelatedContacts[0].UserName=="Gijs")
        }

        /*
        public bool SetUserPreferences()
        {
            return true;
        }

        
        public UserPreferences GetUserPreferences()
        {
            
        }
        */

        /// <summary>
        /// Checks if user is authorized to access this resource.
        /// </summary>
        /// <returns>UserId of the logged in user</returns>
        internal static string CheckAutherization()
        {
            
            if (HttpContext.Current.Session["user"] == null)
                throw new Exception("Your session expired. Login again.");

            var actualUserId = ((User) HttpContext.Current.Session["user"]).Id;
            return actualUserId;
            return null;
        }
    }
}
