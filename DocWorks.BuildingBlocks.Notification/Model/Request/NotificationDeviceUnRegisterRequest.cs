using DocWorks.BuildingBlocks.Global.Model.Request;

namespace DocWorks.BuildingBlocks.Notification.Model.Request
{
    public class NotificationDeviceUnRegisterRequest : BaseRequest
    {
        public string FcmId { get; set; }
    }
}
