using DocWorks.BuildingBlocks.Global.Model;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.Global.Abstractions
{
    public interface IEventBusMessageProcessor
    {
        Task ProcessMessageAsync(Message message);
    }
}
