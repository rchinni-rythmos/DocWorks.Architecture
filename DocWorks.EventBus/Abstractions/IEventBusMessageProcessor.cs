using Microsoft.Azure.ServiceBus;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.EventBus.Abstractions
{
    public interface IEventBusMessageProcessor
    {
        Task ProcessMessageAsync(Message message);
    }
}
