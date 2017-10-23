using DocWorks.BuildingBlocks.EventBus.Enumerations;
using DocWorks.BuildingBlocks.Global.Enumerations;
using System;

namespace DocWorks.BuildingBlocks.EventBus.Abstractions
{
    public interface IEventHandlerRegistry
    {
        void AddEventHandler(EventName eventName, Type eventHandler);

        Type GetHandlerForEvent(EventName eventName);
        bool HasHandlersForEvent(EventName eventName);
    }
}
