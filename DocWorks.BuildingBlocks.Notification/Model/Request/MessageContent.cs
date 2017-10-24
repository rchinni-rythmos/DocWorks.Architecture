using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace DocWorks.BuildingBlocks.Notification.Model.Request
{
    public class MessageContent
    {
        public string title { get; set; }
        public NotificationMessageBody body { get; set; }
    }
}
