using DocWorks.BuildingBlocks.Global.Model.Request;
using System.ComponentModel.DataAnnotations;


namespace DocWorks.BuildingBlocks.Notification.Model.Request
{
    public class NotificationDeviceRegisterRequest : BaseRequest
    {
        [Required]
        public string FcmId { get; set; }
    }
}
