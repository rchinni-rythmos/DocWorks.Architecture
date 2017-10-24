using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.Notification.Abstractions
{
    public interface INotificationService
    {
        Task<bool> SendMessageToUserAsync(UserMessageRequest message);
        Task<bool> SendMessageToTopicAsync(TopicMessageRequest message);
    }
}
