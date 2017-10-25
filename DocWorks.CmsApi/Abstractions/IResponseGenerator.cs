using DocWorks.BuildingBlocks.Global.Enumerations;
using DocWorks.DataAccess.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocWorks.CMS.Api.Abstractions
{
    public interface IResponseGenerator
    {
        Task<Response> CreateResponseAsync(CmsOperation operation);
    }
}
