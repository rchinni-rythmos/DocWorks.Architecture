using DocWorks.BuildingBlocks.DataAccess.Attributes;
using DocWorks.BuildingBlocks.DataAccess.Entity;

namespace DocWorks.BuildingBlocks.Notification.Entity
{
    [CollectionNameAttribute("UserDevice")]
    public class UserDevices : BaseEntity
    {
        public string[] FcmIds { get; set; }

        public string NotificationKey { get; set; }
    }
}
