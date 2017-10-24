using DocWorks.BuildingBlocks.Global.Model;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.EventBus.Abstractions
{
    public interface IEventBusMessagePublisher
    {
        Task PublishAsync(SedaEvent sedaEvent);
    }
}
