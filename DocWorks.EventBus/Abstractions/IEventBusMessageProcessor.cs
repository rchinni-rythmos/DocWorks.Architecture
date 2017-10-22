using DocWorks.BuildingBlocks.EventBus.Model;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.EventBus.Abstractions
{
    public interface IEventBusMessageProcessor
    {
        Task ProcessMessage(Message message);
    }
}
