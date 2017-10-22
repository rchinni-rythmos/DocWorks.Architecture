using DocWorks.BuildingBlocks.DataAccess.Attributes;

namespace DocWorks.BuildingBlocks.DataAccess.Entity
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
