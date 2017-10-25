using DocWorks.BuildingBlocks.EventBus.Model;
using DocWorks.BuildingBlocks.Global.Abstractions;
using DocWorks.GDocFactory.Entity;
using DocWorks.GDocFactory.Repository;
using DocWorks.GDocFactory.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DocWorks.GDocFactory.EventHandlers
{
    class GDriveCreateProjectEventHandler : IEventHandler<EventHandlerInput>
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

        public async Task<dynamic> Handle(EventHandlerInput eventHandlerInput)
        {
            // TODO - review Entity and Model design for EventHandlerInputs
            dynamic requestObj = eventHandlerInput.PayLoad.Request;
            string projectId = requestObj._id;

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
