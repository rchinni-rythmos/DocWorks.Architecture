using DocWorks.BuildingBlocks.DataAccess.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.BuildingBlocks.DataAccess.Entity
{
    public class BaseEntity
    {
        public string _id { get; set; }
        public EntityStatus Status { get; set; }
    }
}
