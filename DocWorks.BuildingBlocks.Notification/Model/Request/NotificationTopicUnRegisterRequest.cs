using DocWorks.BuildingBlocks.Global.Model.Request;

namespace DocWorks.BuildingBlocks.Notification.Model.Request
{
    public class NotificationTopicUnRegisterRequest : BaseRequest
    {
        // TODO: Defect in unregistration. If user is on multiple devices on same screen, based on current design
        // on unregister we are de-registering the user from all devices. 
        public string FcmId { get; set; }
        public string TopicName { get; set; }
    }
}
