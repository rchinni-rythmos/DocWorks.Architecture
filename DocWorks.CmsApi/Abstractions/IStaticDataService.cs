using DocWorks.CMS.Api.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocWorks.CMS.Api.Abstractions
{
    public interface IStaticDataService
    {
        List<StaticDataResponse> GetStaticData(string fieldName);
    }
}
