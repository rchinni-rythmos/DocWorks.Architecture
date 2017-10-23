using DocWorks.BuildingBlocks.EventBus.Model;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.EventBus.Abstractions
{
    public interface IEventHandler<in TEventHandlerInput>
        where TEventHandlerInput : EventHandlerInput
    {
        Task<dynamic> Handle(TEventHandlerInput eventHandlerInput);
    }
}
