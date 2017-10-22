using DocWorks.BuildingBlocks.Notification.Abstractions;
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

        public NotificationController(IRegistrationService registrationService)
        {
            this._registrationService = registrationService;
        }

        [Route("TopicRegister")]
        [HttpPost]
        public async Task<IActionResult> TopicRegisterAsync([FromBody]TopicRegisterRequest register)
        {
            await this._registrationService.TopicRegisterAsync(register, this.UserId);
            return this.Ok();
        }

        [Route("TopicUnRegister")]
        [HttpPost]
        public async Task<IActionResult> TopicUnRegisterAsync([FromBody]TopicUnRegisterRequest unRegister)
        {

            await this._registrationService.TopicUnRegisterAsync(unRegister, this.UserId);
            return this.Ok();
        }

        [Route("DeviceRegister")]
        [HttpPost]
        public async Task<IActionResult> DeviceRegisterAsync([FromBody]DeviceRegisterRequest register)
        {
            await this._registrationService.DeviceRegisterAsync(register, this.UserId);
            return this.Ok();
        }

        [Route("DeviceUnRegister")]
        [HttpPost]
        public async Task<IActionResult> DeviceUnRegisterAsync([FromBody]DeviceUnRegisterRequest unRegister)
        {
            await this._registrationService.DeviceUnRegisterAsync(unRegister, this.UserId);
            return this.Ok();
        }
    }
}
