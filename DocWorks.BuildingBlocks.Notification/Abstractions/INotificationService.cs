using DocWorks.BuildingBlocks.Notification.Model.Request;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.Notification.Abstractions
{
    public interface INotificationService
    {
        Task<bool> SendMessageToUserAsync(UserMessageRequest message);
        Task<bool> SendMessageToTopicAsync(TopicMessageRequest message);
    }
}
