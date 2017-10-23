using DocWorks.BuildingBlocks.EventBus.Abstractions;
using DocWorks.BuildingBlocks.EventBus.Configuration;
using DocWorks.BuildingBlocks.EventBus.Model;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

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

            message.UserProperties.Add(ServiceBusConstants.UserPropertyFrom, sedaEvent.From.ToString());
            message.UserProperties.Add(ServiceBusConstants.UserPropertyCmsOperation, sedaEvent.CmsOperation.ToString());
            message.UserProperties.Add(ServiceBusConstants.UserPropertyEventType, sedaEvent.EventType.ToString());
            message.UserProperties.Add(ServiceBusConstants.UserPropertyPriority, sedaEvent.Priority.ToString());
            message.UserProperties.Add(ServiceBusConstants.UserPropertyEventIndexInFlowMap, sedaEvent.EventIndexInFlowMap);
            message.UserProperties.Add(ServiceBusConstants.UserPropertyEventName, sedaEvent.EventName.ToString());

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
