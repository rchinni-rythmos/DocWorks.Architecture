using DocWorks.BuildingBlocks.ErrorHandling.Model;
using DocWorks.BuildingBlocks.EventBus.Abstractions;
using DocWorks.BuildingBlocks.EventBus.Configuration;
using DocWorks.BuildingBlocks.EventBus.Enumerations;
using DocWorks.BuildingBlocks.EventBus.Model;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.EventBus.Implementation
{
    public class EventBusServiceBus : IEventBus
    {
        private readonly SubscriptionClient _subscriptionClient;
        private readonly ITopicClient _topicClient;
        private readonly AzureServiceBusSettings _azureServiceBusSettings;
        private readonly IEventHandlerRegistry _eventHandlerRegistry;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        const string UserPropertyFrom = "FR";
        const string UserPropertyCmsOperation = "CMSOP";
        const string UserPropertyEventType = "MT";
        const string UserPropertyPriority = "PR";
        const string UserPropertyEventIndexInFlowMap = "EI";
        const string UserPropertyEventName = "APIOP";

        public EventBusServiceBus(AzureServiceBusSettings azureServiceBusSettings, IEventHandlerRegistry eventHandlerRegistry,
            IServiceProvider serviceProvider, ILogger logger)
        {
            this._azureServiceBusSettings = azureServiceBusSettings;
            this._eventHandlerRegistry = eventHandlerRegistry;
            this._serviceProvider = serviceProvider;
            this._logger = logger;

            this._topicClient = new TopicClient(_azureServiceBusSettings.ServiceBusConnectionString,
                _azureServiceBusSettings.TopicName);

            // CMS Api only sends messages. So Subscription would be empty
            if (!string.IsNullOrEmpty(_azureServiceBusSettings.SubscriptionName))
            {
                this._subscriptionClient = new SubscriptionClient(_azureServiceBusSettings.ServiceBusConnectionString,
                    _azureServiceBusSettings.TopicName,
                    _azureServiceBusSettings.SubscriptionName);
            }
        }

        public async Task PublishAsync(SedaEvent sedaEvent)
        {
            var message = new Message() {
                CorrelationId = sedaEvent.ResponseId,
                To = sedaEvent.To.ToString() };

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
                    sedaEvent.From = (SedaService)Enum.Parse(typeof(SedaService), message.UserProperties[UserPropertyFrom] as string);
                    sedaEvent.CmsOperation = (CmsOperation)Enum.Parse(typeof(CmsOperation), message.UserProperties[UserPropertyCmsOperation] as string);
                    sedaEvent.EventType = (EventType)Enum.Parse(typeof(EventType), message.UserProperties[UserPropertyEventType] as string);
                    sedaEvent.Priority = (Priority)Enum.Parse(typeof(Priority), message.UserProperties[UserPropertyPriority] as string);
                    sedaEvent.EventName = (EventName)Enum.Parse(typeof(EventName), message.UserProperties[UserPropertyEventName] as string);
                    sedaEvent.EventIndexInFlowMap = Int32.Parse(message.UserProperties[UserPropertyEventIndexInFlowMap].ToString());
                    var messageData = Encoding.UTF8.GetString(message.Body);
                    sedaEvent.PayLoad = JsonConvert.DeserializeObject<BasePayLoad>(messageData);

                    await ProcessEvent(sedaEvent);
                    // Complete the message so that it is not received again.
                    await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                },
               new MessageHandlerOptions(ExceptionReceivedHandler) { MaxConcurrentCalls = this._azureServiceBusSettings.MaxConcurrentCalls, AutoComplete = false });
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }

        private async Task ProcessEvent(SedaEvent sedaEvent)
        {
            Type eventHandlerType = null;
            var responseSedaEvent = sedaEvent;

            if (_eventHandlerRegistry.HasHandlersForEvent(sedaEvent.EventName))
            {
                try
                {
                    var eventHandlerInput = new EventHandlerInput()
                    {
                        ResponseId = sedaEvent.ResponseId,
                        EventIndexInFlowMap = sedaEvent.EventIndexInFlowMap,
                        PayLoad = (EventTypeRequestPayLoad)sedaEvent.PayLoad
                    };

                    eventHandlerType = _eventHandlerRegistry.GetHandlerForEvent(sedaEvent.EventName);
                    var eventHandlerInstance = this._serviceProvider.GetService(eventHandlerType);
                    var concreteType = typeof(ISedaEventHandler<>).MakeGenericType(eventHandlerType);
                    dynamic eventHandlerResult = await (Task<ExpandoObject>)concreteType.GetMethod("Handle").Invoke(eventHandlerInstance, new object[] { eventHandlerInput });

                    EventTypeResponseSuccessPayLoad responsePayLoad = new EventTypeResponseSuccessPayLoad();
                    responsePayLoad.Request = sedaEvent.PayLoad.Request;
                    responsePayLoad.Response = eventHandlerResult;

                    responseSedaEvent.PayLoad = responsePayLoad;
                    responseSedaEvent.EventType = EventType.ResponseSuccess;
                    responseSedaEvent.Priority = Priority.One;
                    responseSedaEvent.To = SedaService.Notification;
                    await this.PublishAsync(responseSedaEvent);

                }
                catch (Exception ex)
                {
                    // The event handlers would retry (Retry policies to be configured - TODO) the operations on the message
                    // Will throw an exception when they cannot process the message
                    // So we are not depending on message to be re-processed. 

                    var exceptionDetail = new ExceptionDetail()
                    {
                        ExceptionMessage = ex.Message,
                        StackTrace = ex.StackTrace,
                        InnerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : null,
                        InnerExceptionStackTrace = ex.InnerException != null ? ex.InnerException.StackTrace : null
                    };

                    var errorResponse = new ErrorResponse()
                    {
                        Message = "Error occured in execution of: " + eventHandlerType.ToString(),
                        ExceptionDetail = exceptionDetail
                    };

                    var errorResponsePayLoad = new EventTypeResponseFailurePayLoad()
                    {
                        Failure = errorResponse
                    };

                    responseSedaEvent.PayLoad = errorResponsePayLoad;
                    responseSedaEvent.EventType = EventType.ResponseFailure;
                    responseSedaEvent.Priority = Priority.One;
                    responseSedaEvent.To = SedaService.Notification;
                    await this.PublishAsync(responseSedaEvent);
                }
            }
            else
            {
                // TODO log warning
            }
        }

        private void CompressMessageBody()
        {
            throw new NotImplementedException();
            // https://chris.59north.com/post/Compressing-messages-for-the-Windows-Azure-Service-Bus
        }

        private void DecompressMessageBody()
        {
            throw new NotImplementedException();
        }
    }
}
