using DocWorks.BuildingBlocks.Global.Enumerations.Notification;
using System.ComponentModel.DataAnnotations;

namespace DocWorks.CMS.Api.Model.Request
{
    public class TopicRegisterRequest 
    {
        [Required]
        public NotificationTopic TopicName { get; set; }
    }
}
