using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DocWorks.BuildingBlocks.EventBus.Model;
using DocWorks.BuildingBlocks.Global.Abstractions;
using DocWorks.GDocFactory.Entity;
using DocWorks.GDocFactory.Repository;
using DocWorks.GDocFactory.Services;
using Google.Apis.Logging;

namespace DocWorks.GDocFactory.EventHandlers
{
    class GDriveCreateDistributionEventHandler : IEventHandler<EventHandlerInput>
    {
        private readonly IGDriveClient _gdriveClient;
        private readonly ILogger _logger;
        private readonly IGDriveProjectRepository _gDriveProjectRepository;

        public GDriveCreateDistributionEventHandler(IGDriveClient gdriveClient, ILogger logger, IGDriveProjectRepository gDriveProjectRepository)
        {
            this._gdriveClient = gdriveClient;
            this._logger = logger;
            this._gDriveProjectRepository = gDriveProjectRepository;
        }

        public async Task<dynamic> Handle(EventHandlerInput eventHandlerInput)
        {
            // TODO - review Entity and Model design for EventHandlerInputs
            dynamic requestObj = eventHandlerInput.PayLoad.Request;
            string gDriveProjectId = requestObj._id;
            string distributionId = requestObj.distributionId;

            GDriveProject objGDriveProject = await this._gDriveProjectRepository.GetDocumentAsync(gDriveProjectId);
            string parentFolderId= objGDriveProject.GDriveId;

            var distributionFolderId = this._gdriveClient.CreateChildFolder(distributionId, parentFolderId);

            Distribution newDistribution = new Distribution
            {
                DistributionId = Guid.NewGuid().ToString(),
                DistributionGDriveId = distributionFolderId,
            };

            objGDriveProject.DistributionList.Add(newDistribution);
            await this._gDriveProjectRepository.ReplaceElementAsync(gDriveProjectId, objGDriveProject);
            return objGDriveProject;
        }
    }
}
