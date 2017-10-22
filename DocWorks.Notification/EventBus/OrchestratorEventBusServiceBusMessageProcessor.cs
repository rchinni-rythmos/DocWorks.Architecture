using DocWorks.BuildingBlocks.Global.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.ServiceBus;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DocWorks.BuildingBlocks.Global.Model;
using Newtonsoft.Json;
using DocWorks.BuildingBlocks.Global.Enumerations;
using DocWorks.BuildingBlocks.Global.Configuration;
using DocWorks.BuildingBlocks.DataAccess.Abstractions.Repository;
using DocWorks.BuildingBlocks.DataAccess.Entity;
using DocWorks.BuildingBlocks.Notification.Abstractions;

namespace DocWorks.Notification.EventBus
{
    public class OrchestratorEventBusServiceBusMessageProcessor : IEventBusMessageProcessor
    {
        private readonly IEventBusMessagePublisher _messagePublisher;
        private readonly ILogger _logger;
        private readonly IResponseRepository _responseRepository;
        private readonly INotificationService _notificationService;

        public OrchestratorEventBusServiceBusMessageProcessor(ILogger logger, IEventBusMessagePublisher messagePublisher,
            IResponseRepository responseRepository, INotificationService notificationService)
        {
            this._logger = logger;
            this._messagePublisher = messagePublisher;
            this._responseRepository = responseRepository;
            this._notificationService = notificationService;
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

            var responseObject = await this._responseRepository.GetDocumentAsync(sedaEvent.ResponseId);
            var flowMap = responseObject.FlowMap;

            switch (sedaEvent.EventType)
            {
                case EventType.Request:
                    // Need to raise the first set of events from the Map
                    var nextEventSet = flowMap.GetNextSetofEvents(-1);
                    foreach (var eventToRaise in nextEventSet)
                    {
                        var eventMessage = new CMSMessage(
                            currentCmsMessage.CMSResponseId,
                            eventToRaise.To,
                            SEDAService.Notification,
                            MessageType.Request,
                            currentCmsMessage.CMSOperation,
                            currentCmsMessage.CMSPriority,
                            currentCmsMessage.CMSMessageBody,
                            eventToRaise.ApiOperation,
                            eventToRaise.Index
                            );
                        // Set the Event Status to Wait
                        await this._responseRepository.UpdateSpecificElementByFilterAsync(
                            x => x._id == currentCmsMessage.CMSResponseId && x.FlowMap.Events.Any(y => y.Index == eventToRaise.Index),
                            x => x.FlowMap.Events[-1].Status,
                            CMSEventStatus.Wait);
                        await this._messagePublisher.PublishAsync(eventMessage);
                    }
                    break;
                case EventType.ResponseSuccess:
                    // Update the current event status in DB
                    await this._responseRepository.UpdateSpecificElementByFilterAsync(
                            x => x._id == currentCmsMessage.CMSResponseId && x.FlowMap.Events.Any(y => y.Index == currentCmsMessage.CMSEventIndex),
                            x => x.FlowMap.Events[-1].Status,
                            CMSEventStatus.Success);
                    // Get the latest response status - Is the response already marked as complete,
                    // this can happen if a failure occurred with one of the other parallel events within the group.
                    // If so, dont need to do anything, just return
                    if (responseObject.Status == Core.Common.DataAccess.EntityStatus.Error)
                        break;
                    // Check if the entire operation is complete (current one could be the last pending event)
                    // if so, update the response status to OK and add the response to response content
                    // send FCM notification that operation is complete with success
                    // return 
                    // ?? ToDo: multiple success events in parallel (See if only one thread can update the response status through MongoDB findAndModify()
                    // and only that thread raises the FCM notification
                    var latestResponseObj = await this._responseRepository.GetDocumentAsync(currentCmsMessage.CMSResponseId);
                    if (latestResponseObj.FlowMap.IsOperationComplete)
                    {
                        latestResponseObj.Content = currentCmsMessage.CMSMessageBody;
                        latestResponseObj.Status = Core.Common.DataAccess.EntityStatus.Ok;
                        await this._responseRepository.ReplaceElementAsync(currentCmsMessage.CMSResponseId, latestResponseObj);
                        await SendOperationCompleteNotificationAsync(latestResponseObj);
                        break;
                    }
                    // If the whole operation is not complete, check if all events from the current Event Group are complete 
                    // if so, raise the next set of events. 
                    // TODO: (?? multiple success events in parallel within the same group. 
                    // Need to have a flag to mark the group as complete, whichever thread gets that update will raise next set of events)
                    if (latestResponseObj.FlowMap.GetCompleteStatusForEventGroupByEventIndex(currentCmsMessage.CMSEventIndex))
                    {
                        nextEventSet = flowMap.GetNextSetofEvents(currentCmsMessage.CMSEventIndex);
                        foreach (var eventToRaise in nextEventSet)
                        {
                            var eventMessage = new CMSMessage(
                                currentCmsMessage.CMSResponseId,
                                eventToRaise.To,
                                SEDAService.Notification,
                                MessageType.Request,
                                currentCmsMessage.CMSOperation,
                                currentCmsMessage.CMSPriority,
                                currentCmsMessage.CMSMessageBody,
                                eventToRaise.ApiOperation,
                                eventToRaise.Index
                                );
                            // Set the Event Status to Wait
                            await this._responseRepository.UpdateSpecificElementByFilterAsync(
                                x => x._id == currentCmsMessage.CMSResponseId && x.FlowMap.Events.Any(y => y.Index == eventToRaise.Index),
                                x => x.FlowMap.Events[-1].Status,
                                CMSEventStatus.Wait);
                            await MessagingHelper.SendMessageAsync(eventMessage);
                        }
                    }
                    break;
                case EventType.ResponseFailure:
                    // update the DB status for the event
                    await this._responseRepository.UpdateSpecificElementByFilterAsync(
                            x => x._id == currentCmsMessage.CMSResponseId && x.FlowMap.Events.Any(y => y.Index == currentCmsMessage.CMSEventIndex),
                            x => x.FlowMap.Events[-1].Status,
                            CMSEventStatus.Failure);
                    // assumption: Abort the operation on the first failure. That is:
                    // On first failure, update the operation status to Error
                    // and send the FCM notification for complete with failure
                    // TODO: If multiple parallel events are executing in a taskgroup, 
                    // the one or more events could fail and others could be successful
                    // should not be sending multiple failure complete notifications in that case.
                    // Get the response status, if it is not Error, then this is the first error
                    // Update the response status to Error, if that is successful
                    // raise the FCM notification for complete with failure
                    var failResponseObj = await this._responseRepository.GetDocumentAsync(currentCmsMessage.CMSResponseId);
                    if (failResponseObj.Status != Core.Common.DataAccess.EntityStatus.Error)
                    {
                        failResponseObj.Status = Core.Common.DataAccess.EntityStatus.Error;
                        await this._responseRepository.ReplaceElementAsync(currentCmsMessage.CMSResponseId, failResponseObj);
                        await SendOperationCompleteNotificationAsync(failResponseObj);
                        break;
                    }
                    break;
            }
        }

        public async static Task<bool> SendOperationCompleteNotificationAsync(Response responseObj)
        {
            NotificationMessageBody fcmMessageBody = new NotificationMessageBody(); ;
            bool respValue = false;
            HttpResponseMessage httpResponseMessage = null;

            HttpClient httpClient = new HttpClient();
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            switch (responseObj.FlowMap.NotificationType)
            {
                case NotificationType.Targeted:
                    var userMessageRequest = new UserMessageRequest();
                    userMessageRequest.UserId = responseObj.UserId;
                    userMessageRequest.ResponseId = responseObj._id;

                    userMessageRequest.MessageContent = new MessageContent();
                    userMessageRequest.MessageContent.title = responseObj.FlowMap.CMSOperation + " complete";
                    fcmMessageBody.NotificationType = NotificationType.Targeted;
                    fcmMessageBody.NotificationTopic = NotificationTopic.NA;
                    fcmMessageBody.ResponseId = responseObj._id;
                    fcmMessageBody.CMSOperation = responseObj.FlowMap.CMSOperation;
                    userMessageRequest.MessageContent.body = fcmMessageBody;
                    httpResponseMessage = await httpClient.PostJsonAsync<UserMessageRequest>(NotificationReceiverConfigSettings.UrlSendMessageToUser, userMessageRequest);
                    if (httpResponseMessage.IsSuccessStatusCode)
                        respValue = true;
                    break;
                case NotificationType.Broadcast:
                    var topicMessageRequest = new TopicMessageRequest();
                    topicMessageRequest.ResponseId = responseObj._id;
                    topicMessageRequest.TopicPattern = responseObj.FlowMap.NotificationTopic;

                    topicMessageRequest.MessageContent = new MessageContent();
                    topicMessageRequest.MessageContent.title = responseObj.FlowMap.CMSOperation + " complete";
                    fcmMessageBody.NotificationType = NotificationType.Broadcast;
                    fcmMessageBody.NotificationTopic = responseObj.FlowMap.NotificationTopic;
                    fcmMessageBody.ResponseId = responseObj._id;
                    fcmMessageBody.CMSOperation = responseObj.FlowMap.CMSOperation;
                    topicMessageRequest.MessageContent.body = fcmMessageBody;
                    httpResponseMessage = await httpClient.PostJsonAsync<TopicMessageRequest>(NotificationReceiverConfigSettings.UrlSendMessageToTopic, topicMessageRequest);
                    if (httpResponseMessage.IsSuccessStatusCode)
                        respValue = true;
                    break;
            }

            return respValue;
        }
    }
}
