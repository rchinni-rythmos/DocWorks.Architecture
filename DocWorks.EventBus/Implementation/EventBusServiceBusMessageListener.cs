using DocWorks.BuildingBlocks.EventBus.Abstractions;
using DocWorks.BuildingBlocks.EventBus.Configuration;
using DocWorks.BuildingBlocks.Global.Enumerations;
using DocWorks.BuildingBlocks.Global.Enumerations.Events;
using DocWorks.BuildingBlocks.Global.Model;
using DocWorks.BuildingBlocks.Global.Model.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.EventBus.Implementation
{
    public class EventBusServiceBusMessageListener : IEventBusMessageListener
    {
        private readonly SubscriptionClient _subscriptionClient;
        private readonly AzureServiceBusSettings _azureServiceBusSettings;
        private readonly ILogger _logger;
        private readonly IEventBusMessageProcessor _messageProcessor;

        public EventBusServiceBusMessageListener(AzureServiceBusSettings azureServiceBusSettings,
            ILogger logger, IEventBusMessageProcessor messageProcessor)
        {
            this._azureServiceBusSettings = azureServiceBusSettings;
            this._messageProcessor = messageProcessor;
            this._logger = logger;

            this._subscriptionClient = new SubscriptionClient(_azureServiceBusSettings.ServiceBusConnectionString,
                _azureServiceBusSettings.TopicName,
                _azureServiceBusSettings.SubscriptionName);
        }

        public void RegisterEventListener()
        {
            this.RegisterSubscriptionClientMessageHandler();
        }

        private void RegisterSubscriptionClientMessageHandler()
        {
            _subscriptionClient.RegisterMessageHandler(
                async (message, token) =>
                {
                    SedaEvent sedaEvent = new SedaEvent();
                    sedaEvent.ResponseId = message.CorrelationId;
                    sedaEvent.To = (SedaService)Enum.Parse(typeof(SedaService), message.To);
                    sedaEvent.From = (SedaService)Enum.Parse(typeof(SedaService), message.UserProperties[ServiceBusConstants.UserPropertyFrom] as string);
                    sedaEvent.CmsOperation = (CmsOperation)Enum.Parse(typeof(CmsOperation), message.UserProperties[ServiceBusConstants.UserPropertyCmsOperation] as string);
                    sedaEvent.EventType = (EventType)Enum.Parse(typeof(EventType), message.UserProperties[ServiceBusConstants.UserPropertyEventType] as string);
                    sedaEvent.Priority = (Priority)Enum.Parse(typeof(Priority), message.UserProperties[ServiceBusConstants.UserPropertyPriority] as string);
                    sedaEvent.EventName = (EventName)Enum.Parse(typeof(EventName), message.UserProperties[ServiceBusConstants.UserPropertyEventName] as string);
                    sedaEvent.EventIndexInFlowMap = Int32.Parse(message.UserProperties[ServiceBusConstants.UserPropertyEventIndexInFlowMap].ToString());
                    var messageData = Encoding.UTF8.GetString(message.Body);
                    sedaEvent.PayLoad = JsonConvert.DeserializeObject<BasePayLoad>(messageData);

                    await this._messageProcessor.ProcessMessageAsync(sedaEvent);
                    // Complete the message so that it is not received again.
                    await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                },
               new MessageHandlerOptions(ExceptionReceivedHandler) { MaxConcurrentCalls = this._azureServiceBusSettings.MaxConcurrentCalls, AutoComplete = false });
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            // TODO - Code should not reach here, Write code to handle if reaches.
            _logger.LogError($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            _logger.LogError("Exception context for troubleshooting:");
            _logger.LogError($"- Endpoint: {context.Endpoint}");
            _logger.LogError($"- Entity Path: {context.EntityPath}");
            _logger.LogError($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
