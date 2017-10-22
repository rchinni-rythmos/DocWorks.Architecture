using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.BuildingBlocks.EventBus.Abstractions
{
    public interface IEventBusMessageListener
    {
        void RegisterEventListener();
    }
}
