using DocWorks.Core.Common.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocWorks.CMS.Api.Model.Request
{
    public class CreateProjectRequest 
    {
        public string ProjectName { get; set; }
        public string RepoUrl { get; set; }
        public string Description { get; set; }
        public TypeOfContent TypeOfContent { get; set; }
    }
}
