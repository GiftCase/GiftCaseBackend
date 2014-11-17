using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using com.shephertz.app42.paas.sdk.csharp.pushNotification;

namespace GiftCaseBackend.Models
{
    /// <summary>
    /// List of all notifications GiftCase backend can send to the client app
    /// </summary>
    public static class NotificationSettings
    {
        public static string GiftReceived = "{0} sent you {1}!"; // 0 - username of the user who sent the gift, 1 - what gift he sent

        public static void SendGiftReceivedMessage(User userGettingTheGift, User userGivingTheGift, Gift gift)
        {
            PushNotification pushNotification = BaaS.PushNotificationService.SendPushMessageToUser(userGettingTheGift.UserName, 
                string.Format(GiftReceived, userGivingTheGift.UserName, gift.Item.Name));     
        }
    }
}