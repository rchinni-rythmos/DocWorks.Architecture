using DocWorks.BuildingBlocks.EventBus.Abstractions;
using DocWorks.BuildingBlocks.EventBus.Events;
using DocWorks.BuildingBlocks.EventBus.Model;
using DocWorks.GDocFactory.Services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace DocWorks.GDocFactory.EventHandlers
{
    class GDriveCreateProjectEventHandler : ISedaEventHandler<EventHandlerInput>
    {
        private readonly IGDriveClient _gdriveClient;
        public GDriveCreateProjectEventHandler(IGDriveClient gdriveClient)
        {
            this._gdriveClient = gdriveClient;
        }
        public async Task<ExpandoObject> Handle(EventHandlerInput eventHandlerInput)
        {
            

            await Task.Yield();

            return response;
        }
    }
}
