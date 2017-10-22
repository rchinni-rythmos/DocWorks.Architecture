using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.BuildingBlocks.Notification.Abstractions
{
    public interface IRegistrationService
    {
        Task<bool> TopicRegisterAsync(TopicRegistrationRequest register);
        Task<bool> TopicUnRegisterAsync(TopicUnRegistrationRequest unRegister);
        Task<bool> DeviceRegisterAsync(DeviceRegisterRequest register);
        Task<bool> DevicUnRegisterAsync(DeviceUnRegisterRequest unregister);
    }
}
