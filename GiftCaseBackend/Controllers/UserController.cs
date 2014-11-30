﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.pushNotification;
using com.shephertz.app42.paas.sdk.csharp.social;
using GiftCaseBackend.Models;

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




        /// <summary>
        /// Links facebook account with BaaS
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
                    UserName = social.GetUserName(), FacebookAccessToken = accessToken,
                    //Gender = social.GetFacebookProfile().,
                    //Friends = Contacts(userId).ToList(),
                    Status = UserStatus.Registered,
                    ImageUrl = social.facebookProfile.GetPicture()
                };
            }
            catch (Exception ex)
            {
                //SocialExceptionHandling(ex);
                user = new User()
                {
                    Id = "abcd",
                    UserName = userId,
                    FacebookAccessToken = accessToken,
                    //Friends = Contacts(userId).ToList(),
                    Status = UserStatus.Registered,
                };
            }

            try { var push = BaaS.PushNotificationService.StoreDeviceToken(userId, deviceToken, DeviceType.ANDROID); }
            catch(Exception e){}
            return user;
        }
        /// <summary>
        /// URL example:
        /// http://giftcase.azurewebsites.net/api/User/Contacts?userId=ana
        /// http://giftcase.azurewebsites.net/api/User/ana/Contacts
        /// </summary>
        /// <param name="userId">id of the user</param>
        /// <returns>list of friends as JSON or XML</returns>
        [HttpGet]
        [Route("api/User/{userId}/Contacts")]
        [Route("api/User/Contacts")]
        public IEnumerable<Contact> Contacts(string userId)
        {
            try
            {
                var social =BaaS.SocialService.GetFacebookFriendsFromLinkUser(userId);
                return social.GetFriendList().Select(x=>
                    new Contact()
                    {
                        UserName = x.name,
                        ImageUrl = x.GetPicture(),
                        Status = UserStatus.NonRegistered
                    });
            }
            catch(App42Exception ex)
            {
                // for testing purposes
                return TestRepository.Friends;
            }
        }

        /// <summary>
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
            return true;
        }


        /// <summary>
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
        public User Details(string userId)
        {
            return new User()
                {
                    Id = userId,
                    UserName = "ana",
                    FacebookAccessToken = "gsjvnker",
                    Friends = Contacts("ana").ToList(),
                    Status = UserStatus.Registered,
                };;
        }

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



        public void GetTest()
        {
            AmazonProvider.Test();
        }

     
    }
}
