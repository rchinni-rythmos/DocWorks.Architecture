using DocWorks.BuildingBlocks.DataAccess.Attributes;
using DocWorks.BuildingBlocks.DataAccess.Entity;
using DocWorks.BuildingBlocks.Global.Enumerations;

namespace DocWorks.DataAccess.Common.Entity
{
    [CollectionName("Project")]
    public class Project : BaseEntity
    {
        public string ProjectName { get; set; }
        public string RepoUrl { get; set; }
        public string Description { get; set; }
        public TypeOfContent TypeOfContent { get; set; }
    }
}
