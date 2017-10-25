using System.Collections.Generic;
using DocWorks.BuildingBlocks.DataAccess.Attributes;
using DocWorks.BuildingBlocks.DataAccess.Entity;
using Microsoft.Azure.Amqp.Serialization;

namespace DocWorks.GDocFactory.Entity
{
    [CollectionNameAttribute("GDriveProject")]
    public class GDriveProject : BaseEntity
    {
        public string ProjectId { get; set; }

        public string GDriveId { get; set; }
        
        public List<Distribution> DistributionList { get; set; }
    }
}