using DocWorks.BuildingBlocks.Global.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.Global.Abstractions
{
    public interface IEventBusMessagePublisher
    {
        Task PublishAsync(SedaEvent sedaEvent);
    }
}
