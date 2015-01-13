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
        public static string GiftOpened = "{0} just opened your gift of {1}!"; // 0 - username of the user who got the gift, 1 - what gift
        public static string GiftClaimed = "{0} just opened your gift of {1}!"; // 0 - username of the user who got the gift, 1 - what gift

        public static void SendGiftReceivedMessage(Contact userGettingTheGift, Contact userGivingTheGift, Gift gift)
        {
            try
            {
                PushNotification pushNotification = BaaS.PushNotificationService.SendPushMessageToUser(userGettingTheGift.Id,
                string.Format(GiftReceived, userGivingTheGift.UserName, gift.Item.Name));  
            }
            catch(Exception e)
            {}
        }

        public static void SendGiftOpenedMessage(Gift gift)
        {
            try
            {
                PushNotification pushNotification = BaaS.PushNotificationService.SendPushMessageToUser(gift.UserWhoGaveTheGift.Id,
                string.Format(GiftOpened, gift.UserWhoReceivedTheGift.UserName, gift.Item.Name));
            }
            catch (Exception e)
            { }
        }

        public static void SendGiftClaimedMessage(Gift gift)
        {
            try
            {
                PushNotification pushNotification = BaaS.PushNotificationService.SendPushMessageToUser(gift.UserWhoGaveTheGift.Id,
                string.Format(GiftClaimed, gift.UserWhoReceivedTheGift.UserName, gift.Item.Name));
            }
            catch (Exception e)
            { }
        }
        
    }
}