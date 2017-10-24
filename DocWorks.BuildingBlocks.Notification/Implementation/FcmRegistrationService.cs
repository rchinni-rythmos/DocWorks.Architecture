using DocWorks.BuildingBlocks.Notification.Abstractions;
using DocWorks.BuildingBlocks.Notification.Configuration;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.Notification.Implementation
{
    public class FcmRegistrationService : IRegistrationService
    {
        private readonly FcmAppSettings _fcmAppSettings = null;
        public FcmRegistrationService(FcmAppSettings fcmAppSettings)
        {
            this._fcmAppSettings = fcmAppSettings;
        }

        public async Task<bool> DeviceRegisterAsync(DeviceRegisterRequest register)
        {
            string[] tokenarray = new string[] { register.FcmId };
            var url = this._fcmAppSettings.DeviceRegisterUrl;
            dynamic request = new ExpandoObject();//{ registration_tokens= tokenarray };

            var device = await userDeviceRepository.GetDocumentAsync(register.UserId);

            request.operation = device == null ? "create" : string.IsNullOrEmpty(device.NotificationKey) ? "create" : "add"; //register.RegisterEnum.ToString();
            device = device == null ? new UserDevices { _id = register.UserId, FcmIds = new string[] { } } : device;
            //register.UserId;
            request.registration_ids = tokenarray;

            if (request.operation == "add")
            {
                request.notification_key = device.NotificationKey;
                request.notification_key_name = device._id;
            }
            else
            {
                request.notification_key_name = register.UserId;
            }
            //request.to = string.Format("/topics/{0}", unRegister.TopicName);

            //await this.Client.
            this.Client.DefaultRequestHeaders.Add("project_id", this._fcmAppSettings.SenderId);
            var response = await this.Client.PostJsonAsync<ExpandoObject>(url, (ExpandoObject)request);

            if (response.IsSuccessStatusCode)
            {

                var data = await response.Content.ReadAsync<JObject>();
                var NotificationKey = data["notification_key"];
                device.NotificationKey = (string)NotificationKey;

                device.FcmIds = device.FcmIds.Any() ? device.FcmIds.Append(register.FcmId).ToArray() : new string[] { register.FcmId };

                await userDeviceRepository.ReplaceElementAsync(register.UserId, device);

                return true;
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    request.operation = "create";
                    response = await this.Client.PostJsonAsync<ExpandoObject>(url, (ExpandoObject)request);

                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsync<JObject>();
                        var NotificationKey = data["notification_key"];
                        device.NotificationKey = (string)NotificationKey;
                        device.FcmIds.Append(register.FcmId);

                        await userDeviceRepository.ReplaceElementAsync(register.UserId, device);

                        return true;
                    }
                    else
                    {
                        //return false;
                        throw new Exception(await response.Content.ReadAsStringAsync());
                    }
                }
                else
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }

        }

        public async Task<bool> DevicUnRegisterAsync(DeviceUnRegisterRequest register)
        {
            string[] tokenarray = new string[] { register.FcmId };
            var url = this._fcmAppSettings.DeviceRegisterUrl;
            dynamic request = new ExpandoObject();//{ registration_tokens= tokenarray };

            var device = await userDeviceRepository.GetDocumentAsync(register.UserId);

            request.operation = "remove";
            request.notification_key = device.NotificationKey;
            request.notification_key_name = device._id;
            request.registration_ids = tokenarray;
            //request.to = string.Format("/topics/{0}", unRegister.TopicName);

            //await this.Client.
            this.Client.DefaultRequestHeaders.Add("project_id", this._fcmAppSettings.SenderId);
            var response = await this.Client.PostJsonAsync<ExpandoObject>(url, (ExpandoObject)request);

            if (response.IsSuccessStatusCode)
            {
                device.FcmIds = device.FcmIds.Where(x => x != register.FcmId).ToArray();
                if (device.FcmIds.Count() == 0)
                {
                    device.NotificationKey = "";
                }
                else
                {
                    var data = await response.Content.ReadAsync<JObject>();
                    var NotificationKey = data["notification_key"];
                    device.NotificationKey = (string)NotificationKey;
                }

                await userDeviceRepository.ReplaceElementAsync(register.UserId, device);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> TopicRegisterAsync(TopicRegistrationRequest register)
        {
            var device = await userDeviceRepository.GetDocumentAsync(register.UserId);

            // this.Client.DefaultRequestHeaders.Add("project_id", Common.Common.GetSenderID());
            var url = this._fcmAppSettings.TopicRegisterUrl;
            dynamic request = new ExpandoObject();
            request.to = string.Format("/topics/{0}", register.TopicName);

            request.registration_tokens = device.FcmIds;
            var response = await this.Client.PostJsonAsync<ExpandoObject>(url, (ExpandoObject)request);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> TopicUnRegisterAsync(TopicUnRegistrationRequest unRegister)
        {
            var device = await userDeviceRepository.GetDocumentAsync(unRegister.UserId);
            var url = this._fcmAppSettings.TopicUnRegisterUrl;
            dynamic request = new ExpandoObject();
            request.registration_tokens = device.FcmIds;
            request.to = string.Format("/topics/{0}", unRegister.TopicName);

            //await this.Client.

            var response = await this.Client.PostJsonAsync<ExpandoObject>(url, (ExpandoObject)request);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
