﻿using DocWorks.BuildingBlocks.DataAccess.Enumerations;
using DocWorks.BuildingBlocks.EventBus.Abstractions;
using DocWorks.BuildingBlocks.Global.Enumerations.Events;
using DocWorks.BuildingBlocks.Global.Enumerations.Notification;
using DocWorks.BuildingBlocks.Global.Model;
using DocWorks.BuildingBlocks.Notification.Abstractions;
using DocWorks.BuildingBlocks.Notification.Model.Request;
using DocWorks.DataAccess.Common.Abstractions.Repository;
using DocWorks.DataAccess.Common.Entity;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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

        public async Task ProcessMessageAsync(SedaEvent sedaEvent)
        {
            var responseObject = await this._responseRepository.GetDocumentAsync(sedaEvent.ResponseId);
            var flowMap = responseObject.FlowMap;

            switch (sedaEvent.EventType)
            {
                case EventType.Request:
                    // Need to raise the first set of events from the Map
                    var nextEventSet = flowMap.GetNextSetofEvents(-1);
                    foreach (var eventToRaise in nextEventSet)
                    {
                        var eventMessage = new SedaEvent(
                            sedaEvent.ResponseId,
                            EventType.Request,
                            sedaEvent.PayLoad,
                            eventToRaise.EventName,
                            eventToRaise.Index
                            );
                        // Set the Event Status to Wait
                        await this._responseRepository.UpdateSpecificElementByFilterAsync(
                            x => x._id == sedaEvent.ResponseId && x.FlowMap.Events.Any(y => y.Index == eventToRaise.Index),
                            x => x.FlowMap.Events[-1].Status,
                            EventStatus.Wait);
                        await this._messagePublisher.PublishAsync(eventMessage);
                    }
                    break;
                case EventType.ResponseSuccess:
                    // Update the current event status in DB
                    await this._responseRepository.UpdateSpecificElementByFilterAsync(
                            x => x._id == sedaEvent.ResponseId && x.FlowMap.Events.Any(y => y.Index == sedaEvent.EventIndexInFlowMap),
                            x => x.FlowMap.Events[-1].Status,
                            EventStatus.Success);
                    // Get the latest response status - Is the response already marked as complete,
                    // this can happen if a failure occurred with one of the other parallel events within the group.
                    // If so, dont need to do anything, just return
                    if (responseObject.Status == EntityStatus.Error)
                        break;
                    // Check if the entire operation is complete (current one could be the last pending event)
                    // if so, update the response status to OK and add the response to response content
                    // send FCM notification that operation is complete with success
                    // return 
                    // ?? ToDo: multiple success events in parallel (See if only one thread can update the response status through MongoDB findAndModify()
                    // and only that thread raises the FCM notification
                    var latestResponseObj = await this._responseRepository.GetDocumentAsync(sedaEvent.ResponseId);
                    if (latestResponseObj.FlowMap.IsOperationComplete)
                    {
                        latestResponseObj.Content = currentCmsMessage.CMSMessageBody;
                        latestResponseObj.Status = EntityStatus.Ok;
                        await this._responseRepository.ReplaceElementAsync(sedaEvent.ResponseId, latestResponseObj);
                        await SendOperationCompleteNotificationAsync(latestResponseObj);
                        break;
                    }
                    // If the whole operation is not complete, check if all events from the current Event Group are complete 
                    // if so, raise the next set of events. 
                    // TODO: (?? multiple success events in parallel within the same group. 
                    // Need to have a flag to mark the group as complete, whichever thread gets that update will raise next set of events)
                    if (latestResponseObj.FlowMap.GetCompleteStatusForEventGroupByEventIndex(sedaEvent.EventIndexInFlowMap))
                    {
                        nextEventSet = flowMap.GetNextSetofEvents(sedaEvent.EventIndexInFlowMap);
                        foreach (var eventToRaise in nextEventSet)
                        {
                            var eventMessage = new SedaEvent(
                                sedaEvent.ResponseId,
                                EventType.Request,
                                currentCmsMessage.CMSMessageBody,
                                eventToRaise.EventName,
                                eventToRaise.Index
                                );
                            // Set the Event Status to Wait
                            await this._responseRepository.UpdateSpecificElementByFilterAsync(
                                x => x._id == sedaEvent.ResponseId && x.FlowMap.Events.Any(y => y.Index == eventToRaise.Index),
                                x => x.FlowMap.Events[-1].Status,
                                EventStatus.Wait);
                            await this._messagePublisher.PublishAsync(eventMessage);
                        }
                    }
                    break;
                case EventType.ResponseFailure:
                    // update the DB status for the event
                    await this._responseRepository.UpdateSpecificElementByFilterAsync(
                            x => x._id == sedaEvent.ResponseId && x.FlowMap.Events.Any(y => y.Index == sedaEvent.EventIndexInFlowMap),
                            x => x.FlowMap.Events[-1].Status,
                            EventStatus.Failure);
                    // assumption: Abort the operation on the first failure. That is:
                    // On first failure, update the operation status to Error
                    // and send the FCM notification for complete with failure
                    // TODO: If multiple parallel events are executing in a taskgroup, 
                    // the one or more events could fail and others could be successful
                    // should not be sending multiple failure complete notifications in that case.
                    // Get the response status, if it is not Error, then this is the first error
                    // Update the response status to Error, if that is successful
                    // raise the FCM notification for complete with failure
                    var failResponseObj = await this._responseRepository.GetDocumentAsync(sedaEvent.ResponseId);
                    if (failResponseObj.Status != EntityStatus.Error)
                    {
                        failResponseObj.Status = EntityStatus.Error;
                        await this._responseRepository.ReplaceElementAsync(sedaEvent.ResponseId, failResponseObj);
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
