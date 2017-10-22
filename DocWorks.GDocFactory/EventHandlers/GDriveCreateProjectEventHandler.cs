using DocWorks.BuildingBlocks.Global.Abstractions;
using DocWorks.BuildingBlocks.Global.Events;
using DocWorks.BuildingBlocks.Global.Model;
using DocWorks.GDocFactory.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace DocWorks.GDocFactory.EventHandlers
{
    class GDriveCreateProjectEventHandler : IEventHandler<EventHandlerInput>
    {
        private readonly IGDriveClient _gdriveClient;
        private readonly ILogger _logger;
        public GDriveCreateProjectEventHandler(IGDriveClient gdriveClient, ILogger logger)
        {
            this._gdriveClient = gdriveClient;
            this._logger = logger;
        }
        public async Task<ExpandoObject> Handle(EventHandlerInput eventHandlerInput)
        {
            dynamic response = new ExpandoObject();
            response.GDriveProjectFolderID = this._gdriveClient.CreateFolder();
            await Task.Yield();
            return response;
        }
    }
}
