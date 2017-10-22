using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.BuildingBlocks.Global.Abstractions
{
    public interface IEventBusMessageListener
    {
        void RegisterEventListener();
    }
}
