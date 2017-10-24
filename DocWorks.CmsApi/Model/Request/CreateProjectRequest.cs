using DocWorks.BuildingBlocks.Global.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace DocWorks.CMS.Api.Model.Request
{
    public class CreateProjectRequest 
    {
        [Required]
        public string ProjectName { get; set; }
        [Url]
        public string RepoUrl { get; set; }
        public string Description { get; set; }
        [Required]
        public TypeOfContent TypeOfContent { get; set; }
    }
}
