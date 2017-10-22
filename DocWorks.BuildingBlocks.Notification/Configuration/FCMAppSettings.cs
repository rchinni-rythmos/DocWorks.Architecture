using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.Notification.Configuration
{
    public class FcmAppSettings
    {
        public static string GoogleServerKey { get; set; }

        public static string SenderId { get; set; }

        public static string TopicRegisterUrl { get; set; }

        public static string TopicUnRegisterUrl { get; set; }

        public static string DeviceRegisterUrl { get; set; }

        public static string FCMMessageSendingUrl { get; set; }
    }
}
