using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.Notification.Configuration
{
    public class FcmAppSettings
    {
        public string GoogleServerKey { get; set; }

        public string SenderId { get; set; }

        public string TopicRegisterUrl { get; set; }

        public string TopicUnRegisterUrl { get; set; }

        public string DeviceRegisterUrl { get; set; }

        public string FCMMessageSendingUrl { get; set; }
    }
}
