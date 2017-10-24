using DocWorks.BuildingBlocks.EventBus.Abstractions;
using DocWorks.BuildingBlocks.Global.Enumerations.Events;
using System;
using System.Collections.Generic;

namespace DocWorks.BuildingBlocks.Global.Implementation
{
    public class InMemoryEventHandlerRegistry : IEventHandlerRegistry
    {
        Dictionary<EventName, Type> _eventHandlerMap;

        public InMemoryEventHandlerRegistry()
        {
            this._eventHandlerMap = new Dictionary<EventName, Type>();
        }

        public void AddEventHandler(EventName eventName, Type eventHandler)
        {
            if (this._eventHandlerMap.ContainsKey(eventName))
                this._eventHandlerMap.Remove(eventName);

            this._eventHandlerMap.Add(eventName, eventHandler);
        }

        public Type GetHandlerForEvent(EventName eventName)
        {
            return this._eventHandlerMap[eventName];
        }

        public bool HasHandlersForEvent(EventName eventName) => _eventHandlerMap.ContainsKey(eventName);
    }
}
