using DocWorks.BuildingBlocks.Global.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.BuildingBlocks.Global.Abstractions
{
    public interface IEventHandlerRegistry
    {
        void AddEventHandler(EventName eventName, Type eventHandler);

        Type GetHandlerForEvent(EventName eventName);
        bool HasHandlersForEvent(EventName eventName);
    }
}
