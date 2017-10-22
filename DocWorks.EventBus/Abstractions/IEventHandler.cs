using DocWorks.BuildingBlocks.Global.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.Global.Abstractions
{
    public interface IEventHandler<in TEventHandlerInput>
        where TEventHandlerInput : EventHandlerInput
    {
        Task<ExpandoObject> Handle(TEventHandlerInput eventHandlerInput);
    }
}
