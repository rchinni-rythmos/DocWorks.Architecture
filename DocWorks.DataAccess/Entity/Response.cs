using DocWorks.BuildingBlocks.DataAccess.Attributes;
using DocWorks.BuildingBlocks.ErrorHandling.Model;
using System;
using System.Dynamic;

namespace DocWorks.BuildingBlocks.DataAccess.Entity
{
    [CollectionName("Response")]
    public class Response : BaseEntity
    {
        public ExpandoObject Content { get; set; }

        public string UserId { get; set; }

        public FlowMap FlowMap { get; set; }

        public DateTime CreatedOn { get; set; }

        public ErrorResponse ErrorResponse { get ; set ;}
    }
}
