using DocWorks.BuildingBlocks.EventBus.Events;
using DocWorks.BuildingBlocks.EventBus.Model;
using System;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.EventBus.Abstractions
{
    public interface IEventBus
    {
        Task PublishAsync(SedaEvent sedaEvent);

        void RegisterEventListener();
    }
}
