using System;
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
         * How you can access the REST api provided by this controler:
         * use the url:  /api/User/NameOfTheAction?parameterName=value&parameter2=value
         * 
         * Name of the action is the method name. If you want to change the url into something different
         * than the name of the action put an attribute: [ActionName("NewName")] above the method
         * 
         * The results returned by REST api are either in JSON or XML format. The format depends on what the requester
         * specified he would like to receive. If not specified it defaults to JSON.
         * 
         * The server will automaticaly try to map what methods should be called with what type of request. If you
         * want to specify it manualy you can put attributes above the method, eg: [HttpGet],[HttpPost]
         */




        /// <summary>
        /// Links facebook account with BaaS
        /// URL example:
        /// http://localhost:22467/api/User/LoginFacebook?userName=ana&accessToken=someGarbage&deviceToken=whatever
        /// </summary>
        /// <param name="userName">User's user name</param>
        /// <param name="accessToken">Facebook access token</param>
        /// <param name="deviceToken">A token identifying the device user is login in from</param>
        /// <returns>status message</returns>
        [HttpGet]
        public User Login(string userName, string accessToken, string deviceToken)
        {
            try
            {
                var social = BaaS.SocialService.LinkUserFacebookAccount(userName, accessToken);
                var push =  BaaS.PushNotificationService.StoreDeviceToken(userName, deviceToken, DeviceType.ANDROID);

                return new User()
                {
                    Id = accessToken+userName,
                    UserName = userName, FacebookAccessToken = accessToken,
                    Friends = Contacts(userName).ToList(),
                    Status = UserStatus.Registered,
                    ImageUrl = social.facebookProfile.GetPicture()
                };
            }
            catch (Exception ex)
            {
                //SocialExceptionHandling(ex);
                return new User()
                {
                    Id = "abcd",
                    UserName = userName,
                    FacebookAccessToken = accessToken,
                    Friends = Contacts(userName).ToList(),
                    Status = UserStatus.Registered,
                    ImageUrl = "https://lh5.googleusercontent.com/-z4GINoMoCgA/AAAAAAAAAAI/AAAAAAAAABQ/CM0fRlsGcD8/photo.jpg"
                };
            }
        }
        /// <summary>
        /// URL example:
        /// http://localhost:22467/api/User/GetFacebookFriendList?userName=ana
        /// </summary>
        /// <param name="userName">Facebook userName of the user</param>
        /// <returns>list of friends as JSON or XML depending on which type the get call said it preferred</returns>
        [HttpGet]
        public IEnumerable<Contact> Contacts(string userId)
        {
            try
            {
                var social =BaaS.SocialService.GetFacebookProfilesFromIds(new []{userId});
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
                SocialExceptionHandling(ex);

                // for testing purposes
                return TestRepository.Friends;
            }
        }

        

        public bool LogOut(string userId, string deviceToken)
        {
            return true;
        }

        public void GetTest()
        {
            AmazonProvider.Test();
        }

        
        
        /// <summary>
        /// 1400 - BAD REQUEST - The Request parameters are invalid 
        /// 1401 - UNAUTHORIZED - Client is not authorized 
        /// 1500 - INTERNAL SERVER ERROR - Internal Server Error. Please try again 
        /// 3800 - NOT FOUND - Twitter App Credentials(ConsumerKey / ConsumerSecret) does not exist. 
        /// 3802 - NOT FOUND - Twitter User Access Credentials does not exist. Please use linkUserTwitterAccount API to link the User Twitter account. 
        /// 3803 - BAD REQUEST - The Twitter Access Credentials are invalid." + &lt;Exception Message&gt;. 
        /// 3804 - NOT FOUND - Facebook App Credentials(ConsumerKey/ConsumerSecret) does not exist. 
        /// 3805 - BAD REQUEST - The Facebook Access Credentials are invalid + &lt;Received Facebook Exception Message&gt;. 
        /// 3806 - NOT FOUND - Facebook User Access Credentials does not exist. Please use linkUserFacebookAccount API to link the User facebook account. 
        /// 3807 - NOT FOUND - LinkedIn App Credentials(ApiKey/SecretKey) does not exist. 
        /// 3808 - BAD REQUEST - The Access Credentials are invalid + &lt;Exception Message&gt;. 
        /// 3809 - NOT FOUND - LinkedIn User Access Credentials does not exist. Please use linkUserLinkedInAccount API to link the User LinkedIn account. 
        /// 3810 - NOT FOUND - Social App Credentials do not exist. 
        /// 3811 - NOT FOUND - User Social Access Credentials do not exist. Please use linkUserXXXXXAccount API to link the User Social account. 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private string SocialExceptionHandling(App42Exception ex)
        {
            int appErrorCode = ex.GetAppErrorCode();
            int httpErrorCode = ex.GetHttpErrorCode();
            if (appErrorCode == 3800)
            {
                // Handle here for Not Found (Twitter App Credentials(ConsumerKey / ConsumerSecret) does not exist.)  
            }
            else if (appErrorCode == 3801)
            {
                // Handle here for Bad Request (The request is Unauthorized with the provided credentials.)  
            }
            else if (appErrorCode == 3802)
            {
                // Handle here for Not Found (Twitter User Access Credentials does not exist. Please use linkUserTwitterAccount API to link the User Twitter account.)  
            }
            else if (appErrorCode == 3803)
            {
                // Handle here for Bad Request (The Twitter Access Credentials are invalid.)  
            }
            else if (appErrorCode == 1401)
            {
                // handle here for Client is not authorized  
            }
            else if (appErrorCode == 1500)
            {
                // handle here for Internal Server Error  
            }
            return ex.GetMessage();    
        }
    }
}
