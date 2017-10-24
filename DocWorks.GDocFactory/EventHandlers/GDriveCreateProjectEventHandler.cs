using DocWorks.BuildingBlocks.EventBus.Abstractions;
using DocWorks.BuildingBlocks.Global.Model;
using DocWorks.GDocFactory.Entity;
using DocWorks.GDocFactory.Repository;
using DocWorks.GDocFactory.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DocWorks.GDocFactory.EventHandlers
{
    class GDriveCreateProjectEventHandler : IEventHandler<SedaEvent>
    {
        private readonly IGDriveClient _gdriveClient;
        private readonly ILogger _logger;
        private readonly IGDriveProjectRepository _gDriveProjectRepository;
        public GDriveCreateProjectEventHandler(IGDriveClient gdriveClient, ILogger logger, IGDriveProjectRepository gDriveProjectRepository)
        {
            this._gdriveClient = gdriveClient;
            this._logger = logger;
            this._gDriveProjectRepository = gDriveProjectRepository;
        }
        public async Task<dynamic> Handle(SedaEvent eventHandlerInput)
        {
            // TODO - review Entity and Model design for EventHandlerInputs
            dynamic requestObj = eventHandlerInput.PayLoad.Request;
            string projectId = requestObj._id;

            // TODO - Check if record for folder exists in DB, if not create new folder
            var folderId = this._gdriveClient.CreateChildFolderOfRoot(projectId);

            GDriveProject objGDriveProject = new GDriveProject()
            {
                _id = Guid.NewGuid().ToString(),
                GDriveId = folderId,
                ProjectId = projectId,
            };

            await this._gDriveProjectRepository.AddDocumentAsync(objGDriveProject);
            return objGDriveProject;
        }
    }
}
