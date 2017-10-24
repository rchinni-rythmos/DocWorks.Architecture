using DocWorks.BuildingBlocks.Notification.Abstractions;
using DocWorks.BuildingBlocks.Notification.Configuration;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.Notification.Implementation
{
    public class FcmNotificationService : INotificationService
    {
        private readonly FcmAppSettings _fcmAppSettings = null;
        public FcmNotificationService(FcmAppSettings fcmAppSettings)
        {
            this._fcmAppSettings = fcmAppSettings;
        }
        public async Task<bool> SendMessageToUserAsync(UserMessageRequest message)
        {
            var device = await userDeviceRepository.GetDocumentAsync(message.UserId);
            var url = FCMAppSettings.TopicUnRegisterUrl;
            dynamic request = new ExpandoObject();
            request.to = device.NotificationKey;
            request.notification = message.MessageContent;

            var response = await this.Client.PostJsonAsync<ExpandoObject>(this._fcmAppSettings.FCMMessageSendingUrl, (ExpandoObject)request);

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

            var response = await this.Client.PostJsonAsync<ExpandoObject>(this._fcmAppSettings.FCMMessageSendingUrl, (ExpandoObject)request);

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
