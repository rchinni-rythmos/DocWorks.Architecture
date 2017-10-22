using DocWorks.BuildingBlocks.EventBus.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.EventBus.Abstractions
{
    public interface IEventHandler<in TEventHandlerInput>
        where TEventHandlerInput : EventHandlerInput
    {
        Task<ExpandoObject> Handle(TEventHandlerInput eventHandlerInput);
    }
}
