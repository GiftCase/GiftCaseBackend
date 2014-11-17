using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.pushNotification;
using com.shephertz.app42.paas.sdk.csharp.social;
using com.shephertz.app42.paas.sdk.csharp.storage;
using com.shephertz.app42.paas.sdk.csharp.user;

namespace GiftCaseBackend.Models
{
    public static class BaaS
    {
        private const string APIkey = "7fc41ebb0ac5c4c9342bc47ac49e1bc097eb67de7e68828bbdf009d1e5cc26fb";
        private const string SecretKey = "3fd5ebed57cbe849f0915cbe394ad7ebfd15082c24345cd18cb630bcfab02ab5";

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
    }
}