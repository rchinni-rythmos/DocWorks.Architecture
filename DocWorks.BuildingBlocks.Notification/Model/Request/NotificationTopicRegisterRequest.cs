using DocWorks.BuildingBlocks.Global.Model.Request;

namespace DocWorks.BuildingBlocks.Notification.Model.Request
{
    public class NotificationTopicRegisterRequest : BaseRequest
    {
        public string TopicName { get; set; }
    }
}
