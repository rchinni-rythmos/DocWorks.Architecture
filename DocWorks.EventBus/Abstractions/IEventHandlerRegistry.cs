using DocWorks.BuildingBlocks.Global.Enumerations.Events;
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
