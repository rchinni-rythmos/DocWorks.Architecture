using DocWorks.BuildingBlocks.Global.Enumerations.Notification;
using DocWorks.BuildingBlocks.Global.Model.Request;
using System.Dynamic;

namespace DocWorks.BuildingBlocks.Notification.Model.Request
{
    public class TopicMessageRequest : BaseRequest
    {
        public MessageContent MessageContent { get; set; }
        public NotificationTopic TopicPattern { get; set; }        
    }
}
