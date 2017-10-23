using DocWorks.BuildingBlocks.DataAccess.Attributes;
using DocWorks.BuildingBlocks.DataAccess.Entity;

namespace DocWorks.GDocFactory.Entity
{
    [CollectionNameAttribute("GDriveProject")]
    public class GDriveProject : BaseEntity
    {
        public string ProjectId { get; set; }

        public string GDriveId { get; set; }
    }
}
