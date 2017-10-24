using DocWorks.BuildingBlocks.EventBus.Abstractions;
using DocWorks.BuildingBlocks.EventBus.Configuration;
using DocWorks.BuildingBlocks.Global.Enumerations;
using DocWorks.BuildingBlocks.Global.Enumerations.Events;
using DocWorks.BuildingBlocks.Global.Model;
using DocWorks.BuildingBlocks.Global.Model.ErrorHandling;
using DocWorks.BuildingBlocks.Global.Model.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
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

        public async Task ProcessMessageAsync(SedaEvent sedaEvent)
        {
            Type eventHandlerType = null;
            var responseSedaEvent = sedaEvent;

            if (_eventHandlerRegistry.HasHandlersForEvent(sedaEvent.EventName))
            {
                try
                {
                    eventHandlerType = _eventHandlerRegistry.GetHandlerForEvent(sedaEvent.EventName);
                    var eventHandlerInstance = this._serviceProvider.GetService(eventHandlerType);
                    var concreteType = typeof(IEventHandler<>).MakeGenericType(eventHandlerType);
                    dynamic eventHandlerResult = await (Task<ExpandoObject>)concreteType.GetMethod("Handle").Invoke(eventHandlerInstance, new object[] { sedaEvent });

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
