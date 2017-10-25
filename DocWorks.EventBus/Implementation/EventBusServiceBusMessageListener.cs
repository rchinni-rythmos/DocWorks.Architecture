using DocWorks.BuildingBlocks.Global.Abstractions;
using DocWorks.BuildingBlocks.Global.Configuration;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
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
                    await this._messageProcessor.ProcessMessageAsync(message);
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
