using AutoMapper;
using DocWorks.BuildingBlocks.Notification.Abstractions;
using DocWorks.BuildingBlocks.Notification.Model.Request;
using DocWorks.CMS.Api.Model.Request;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocWorks.CMS.Api.Controllers
{
    [Route("api/[controller]")]
    public class NotificationController : Controller
    {
        // TODO: remove hardcoding of UserId after auth integration
        string UserId = "408cf2f7-1676-484b-8f3b-0566f556b2f2";

        private readonly IRegistrationService _registrationService = null;
        private readonly IMapper _iMapper =  null;

        public NotificationController(IRegistrationService registrationService, IMapper iMapper)
        {
            this._registrationService = registrationService;
            this._iMapper = iMapper;
        }

        [Route("TopicRegister")]
        [HttpPost]
        public async Task<IActionResult> TopicRegisterAsync([FromBody]TopicRegisterRequest registerRequest)
        {
            var notificationTopicRegisterRequest = this._iMapper.Map<TopicRegisterRequest, NotificationTopicRegisterRequest>(registerRequest);
            notificationTopicRegisterRequest.UserId = this.UserId;
            await this._registrationService.TopicRegisterAsync(notificationTopicRegisterRequest);
            return this.Ok();
        }

        [Route("TopicUnRegister")]
        [HttpPost]
        public async Task<IActionResult> TopicUnRegisterAsync([FromBody]TopicUnRegisterRequest unRegisterRequest)
        {
            var notificationTopicUnRegisterRequest = this._iMapper.Map<TopicUnRegisterRequest, NotificationTopicUnRegisterRequest>(unRegisterRequest);
            notificationTopicUnRegisterRequest.UserId = this.UserId;
            await this._registrationService.TopicUnRegisterAsync(notificationTopicUnRegisterRequest);
            return this.Ok();
        }

        [Route("DeviceRegister")]
        [HttpPost]
        public async Task<IActionResult> DeviceRegisterAsync([FromBody]DeviceRegisterRequest registerRequest)
        {
            var notificationDeviceRegisterRequest = this._iMapper.Map<DeviceRegisterRequest, NotificationDeviceRegisterRequest>(registerRequest);
            notificationDeviceRegisterRequest.UserId = this.UserId;
            await this._registrationService.DeviceRegisterAsync(notificationDeviceRegisterRequest);
            return this.Ok();
        }

        [Route("DeviceUnRegister")]
        [HttpPost]
        public async Task<IActionResult> DeviceUnRegisterAsync([FromBody]DeviceUnRegisterRequest unRegisterRequest)
        {
            var notificationDeviceUnRegisterRequest = this._iMapper.Map<DeviceUnRegisterRequest, NotificationDeviceUnRegisterRequest>(unRegisterRequest);
            notificationDeviceUnRegisterRequest.UserId = this.UserId;
            await this._registrationService.DeviceUnRegisterAsync(notificationDeviceUnRegisterRequest);
            return this.Ok();
        }
    }
}
