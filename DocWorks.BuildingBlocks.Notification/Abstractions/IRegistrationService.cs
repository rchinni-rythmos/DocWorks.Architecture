using DocWorks.BuildingBlocks.Notification.Model.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.Notification.Abstractions
{
    public interface IRegistrationService
    {
        Task<bool> TopicRegisterAsync(NotificationTopicRegisterRequest registerRequest);
        Task<bool> TopicUnRegisterAsync(NotificationTopicUnRegisterRequest unRegisterRequest);
        Task<bool> DeviceRegisterAsync(NotificationDeviceRegisterRequest registerRequest);
        Task<bool> DeviceUnRegisterAsync(NotificationDeviceUnRegisterRequest unRegisterRequest);
    }
}
