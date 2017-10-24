using DocWorks.BuildingBlocks.Global.Model;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.EventBus.Abstractions
{
    public interface IEventHandler<in TEventHandlerInput>
        where TEventHandlerInput : SedaEvent
    {
        Task<dynamic> Handle(TEventHandlerInput eventHandlerInput);
    }
}
