using DocWorks.BuildingBlocks.EventBus.Model;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.Global.Abstractions
{
    public interface IEventHandler<in TEventHandlerInput>
        where TEventHandlerInput : EventHandlerInput
    {
        Task<dynamic> Handle(TEventHandlerInput eventHandlerInput);
    }
}
