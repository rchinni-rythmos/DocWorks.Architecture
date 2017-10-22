using DocWorks.BuildingBlocks.EventBus.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.EventBus.Abstractions
{
    public interface ISedaEventHandler<in TEventHandlerInput>
        where TEventHandlerInput : EventHandlerInput
    {
        Task<EventTypeResponseSuccessPayLoad> Handle(TEventHandlerInput eventHandlerInput);
    }
}
