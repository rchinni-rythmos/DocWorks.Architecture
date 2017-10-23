using DocWorks.BuildingBlocks.Global.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using DocWorks.BuildingBlocks.Global.Model;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DocWorks.BuildingBlocks.Global.Configuration;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace DocWorks.BuildingBlocks.EventBus.Implementation
{
    public class EventBusServiceBusMessagePublisher : IEventBusMessagePublisher
    {
        private readonly ITopicClient _topicClient;
        private readonly AzureServiceBusSettings _azureServiceBusSettings;

        public EventBusServiceBusMessagePublisher(AzureServiceBusSettings azureServiceBusSettings, ILogger logger)
        {
            this._topicClient = new TopicClient(_azureServiceBusSettings.ServiceBusConnectionString,
    _azureServiceBusSettings.TopicName);
        }

        public async Task PublishAsync(SedaEvent sedaEvent)
        {
            var message = new Message()
            {
                CorrelationId = sedaEvent.ResponseId,
                To = sedaEvent.To.ToString()
            };

            message.UserProperties.Add(UserPropertyFrom, sedaEvent.From.ToString());
            message.UserProperties.Add(UserPropertyCmsOperation, sedaEvent.CmsOperation.ToString());
            message.UserProperties.Add(UserPropertyEventType, sedaEvent.EventType.ToString());
            message.UserProperties.Add(UserPropertyPriority, sedaEvent.Priority.ToString());
            message.UserProperties.Add(UserPropertyEventIndexInFlowMap, sedaEvent.EventIndexInFlowMap);
            message.UserProperties.Add(UserPropertyEventName, sedaEvent.EventName.ToString());

            var jsonString = JsonConvert.SerializeObject(sedaEvent.PayLoad);
            message.Body = Encoding.UTF8.GetBytes(jsonString);

            await this._topicClient.SendAsync(message);
        }

        private void CompressMessageBody()
        {
            throw new NotImplementedException();
            // TODO
            // https://chris.59north.com/post/Compressing-messages-for-the-Windows-Azure-Service-Bus
        }
    }
}
