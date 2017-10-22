using DocWorks.BuildingBlocks.ErrorHandling.Model;
using DocWorks.BuildingBlocks.Global.Abstractions;
using DocWorks.BuildingBlocks.Global.Configuration;
using DocWorks.BuildingBlocks.Global.Enumerations;
using DocWorks.BuildingBlocks.Global.Model;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.Global.Implementation
{
    public class DefaultEventBusServiceBusMessageProcessor : IEventBusMessageProcessor
    {
        private readonly IEventHandlerRegistry _eventHandlerRegistry;
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventBusMessagePublisher _messagePublisher;

        public DefaultEventBusServiceBusMessageProcessor(IEventHandlerRegistry eventHandlerRegistry, IServiceProvider serviceProvider, 
            ILogger logger, IEventBusMessagePublisher messagePublisher)
        {
            this._eventHandlerRegistry = eventHandlerRegistry;
            this._logger = logger;
            this._serviceProvider = serviceProvider;
            this._messagePublisher = messagePublisher;
        }

        public async Task ProcessMessageAsync(Message message)
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
                    var concreteType = typeof(IEventHandler<>).MakeGenericType(eventHandlerType);
                    dynamic eventHandlerResult = await (Task<ExpandoObject>)concreteType.GetMethod("Handle").Invoke(eventHandlerInstance, new object[] { eventHandlerInput });

                    EventTypeResponseSuccessPayLoad responsePayLoad = new EventTypeResponseSuccessPayLoad();
                    responsePayLoad.Request = sedaEvent.PayLoad.Request;
                    responsePayLoad.Response = eventHandlerResult;

                    responseSedaEvent.PayLoad = responsePayLoad;
                    responseSedaEvent.EventType = EventType.ResponseSuccess;
                    responseSedaEvent.Priority = Priority.One;
                    responseSedaEvent.To = SedaService.Notification;
                    await this._messagePublisher.PublishAsync(responseSedaEvent);
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
                    await this._messagePublisher.PublishAsync(responseSedaEvent);
                }
            }
            else
            {
                // TODO log warning
            }
        }

        private void DecompressMessageBody()
        {
            throw new NotImplementedException();
        }
    }
}
