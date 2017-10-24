using DocWorks.BuildingBlocks.Notification.Abstractions;
using DocWorks.BuildingBlocks.Notification.Abstractions.Repository;
using DocWorks.BuildingBlocks.Notification.Configuration;
using DocWorks.BuildingBlocks.Notification.Model.Request;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.Notification.Implementation
{
    public class FcmNotificationService : INotificationService
    {
        private readonly FcmAppSettings _fcmAppSettings = null;
        private readonly IUserDeviceRepository _userDeviceRepository = null;
        private HttpClient _httpClient = null;
        public FcmNotificationService(FcmAppSettings fcmAppSettings, IUserDeviceRepository userDeviceRepository)
        {
            this._fcmAppSettings = fcmAppSettings;
            this._userDeviceRepository = userDeviceRepository;

            this._httpClient = new HttpClient();
            this._httpClient.DefaultRequestHeaders.Accept.Clear();
            this._httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this._httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("key", this._fcmAppSettings.GoogleServerKey);
        }
        public async Task<bool> SendMessageToUserAsync(UserMessageRequest message)
        {
            var device = await this._userDeviceRepository.GetDocumentAsync(message.UserId);
            var url = this._fcmAppSettings.TopicUnRegisterUrl;
            dynamic request = new ExpandoObject();
            request.to = device.NotificationKey;
            request.notification = message.MessageContent;

            var response = await this._httpClient.PostJsonAsync<ExpandoObject>(this._fcmAppSettings.FCMMessageSendingUrl, (ExpandoObject)request);

            //TODO:The Call will return you partial success,the app server should retry with back off between retries. 
            // For Now only checking the Success of the call not the partial success.
            if (response.IsSuccessStatusCode)
            {

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SendMessageToTopicAsync(TopicMessageRequest message)
        {
            dynamic request = new ExpandoObject();
            request.to = string.Format("/topics/{0}", message.TopicPattern);
            request.notification = message.MessageContent;

            var response = await this._httpClient.PostJsonAsync<ExpandoObject>(this._fcmAppSettings.FCMMessageSendingUrl, (ExpandoObject)request);

            //TODO:The Call will return you partial success,the app server should retry with back off between retries. 
            // For Now only checking the Success of the call not the partial success.
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
