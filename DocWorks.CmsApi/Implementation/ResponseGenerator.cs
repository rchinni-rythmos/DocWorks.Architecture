﻿using DocWorks.BuildingBlocks.DataAccess.Enumerations;
using DocWorks.BuildingBlocks.Global.Enumerations;
using DocWorks.CMS.Api.Abstractions;
using DocWorks.DataAccess.Common.Abstractions.Repository;
using DocWorks.DataAccess.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocWorks.CMS.Api.Implementation
{
    
    public class ResponseGenerator : IResponseGenerator
    {
        private readonly IResponseRepository _responseRepository = null;
        private readonly IFlowMapRepository _flowMapRepository = null;
        private IResponseRepository responseRepository;

        public ResponseGenerator(IResponseRepository responseRepository, IFlowMapRepository flowMapRepository)
        {
            this._responseRepository = responseRepository;
            this._flowMapRepository = flowMapRepository;
        }
        public async Task<Response> CreateResponseAsync(CmsOperation operation)
        {
            Response responseObj = new Response();
            responseObj._id = Guid.NewGuid().ToString();
            responseObj.Status = EntityStatus.Wait;
            responseObj.Content = new System.Dynamic.ExpandoObject();
            responseObj.CreatedOn = DateTime.UtcNow;
            // TODO: null handling
            var flowMapForCurrentOperation = this._flowMapRepository.FindAllDocument(fm => fm.CMSOperation == operation)[0];
            responseObj.FlowMap = flowMapForCurrentOperation;
            // TODO: mtonde:remove hard coding,when auth is implemented. And send it from the Controller layer
            responseObj.UserId = "408cf2f7-1676-484b-8f3b-0566f556b2f2";
            await this._responseRepository.AddDocumentAsync(responseObj);

            return responseObj;
        }
    }
}
