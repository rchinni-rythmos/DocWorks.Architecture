using DocWorks.BuildingBlocks.Global.Enumerations;
using DocWorks.CMS.Api.Abstractions;
using DocWorks.CMS.Api.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocWorks.CMS.Api.Implementation
{
    public class StaticDataService : IStaticDataService
    {
        public List<StaticDataResponse> GetStaticData(string fieldName)
        {
            List<StaticDataResponse> ddlValues = new List<StaticDataResponse>();
            switch ((StaticDataType)Enum.Parse(typeof(StaticDataType), fieldName))
            {
                case StaticDataType.TypeOfContent:
                    ddlValues = Enum.GetValues(typeof(TypeOfContent)).Cast<TypeOfContent>().Select(e => new StaticDataResponse { Key = (int)e, Value = e.ToString() }).ToList();
                    break;
            }
            return ddlValues;
        }
    }
}
