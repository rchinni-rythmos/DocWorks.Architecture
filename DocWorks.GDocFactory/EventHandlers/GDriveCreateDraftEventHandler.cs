using DocWorks.BuildingBlocks.EventBus.Model;
using DocWorks.BuildingBlocks.Global.Abstractions;
using DocWorks.GDocFactory.Repository;
using DocWorks.GDocFactory.Services;
using Google.Apis.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DocWorks.GDocFactory.Entity;
using DocWorks.GDocFactory.Model;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace DocWorks.GDocFactory.EventHandlers
{
    class GDriveCreateDraftEventHandler : IEventHandler<EventHandlerInput>
    {
        private readonly IGDriveClient _gdriveClient;
        private readonly ILogger _logger;
        private readonly IGDriveProjectRepository _gDriveProjectRepository;
        private readonly IDataConversion _dataConversion;

        public GDriveCreateDraftEventHandler(IGDriveClient gdriveClient, ILogger logger, IGDriveProjectRepository gDriveProjectRepository, IDataConversion dataConversion)
        {
            this._gdriveClient = gdriveClient;
            this._logger = logger;
            this._gDriveProjectRepository = gDriveProjectRepository;
            this._dataConversion = dataConversion;
        }

        public async Task<dynamic> Handle(EventHandlerInput eventHandlerInput)
        {
            // TODO - review Entity and Model design for EventHandlerInputs
            dynamic requestObj = eventHandlerInput.PayLoad.Request;
            string gDriveProjectId = requestObj._id;
            string distributionId = requestObj.distributionId;
            string draftId = requestObj.draftId;
            string markdownContent = requestObj.nodeFileUrl;

            GDriveProject objGDriveProject = await this._gDriveProjectRepository.GetDocumentAsync(gDriveProjectId);
            string parentFolderId = objGDriveProject.GDriveId;


            string htmlString = _dataConversion.ConvertMarkdownToHtml(markdownContent);

            var newDraftId = this._gdriveClient.CreateDocumentInFolder(draftId, htmlString, parentFolderId);

            //Distribution newDistribution = new Distribution
            //{
            //    DistributionId = Guid.NewGuid().ToString(),
            //    DistributionGDriveId = distributionFolderId,
            //};

            //objGDriveProject.DistributionList.Add(newDistribution);
            //await this._gDriveProjectRepository.ReplaceElementAsync(gDriveProjectId, objGDriveProject);
            return objGDriveProject;
        }
    }
}

