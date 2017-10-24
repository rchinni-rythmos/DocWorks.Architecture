using DocWorks.BuildingBlocks.Global.Enumerations;
using DocWorks.BuildingBlocks.Global.Enumerations.Notification;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DocWorks.BuildingBlocks.Notification.Model.Request
{
    public class NotificationMessageBody
    {
        [JsonProperty("responseId")]
        public string ResponseId { get; set; }
        [JsonProperty("notificationType")]
        public NotificationType NotificationType { get; set; }
        [JsonProperty("notificationTopic")]
        public NotificationTopic NotificationTopic { get; set; }

        [JsonProperty("cmsOperation")]
        [JsonConverter(typeof(StringEnumConverter))]
        public CmsOperation CMSOperation { get; set; }
    }
}
