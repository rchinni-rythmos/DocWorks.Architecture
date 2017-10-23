using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.BuildingBlocks.DataAccess.Indexes
{
    public class DbCollectionIndexCreationOperation
    {
        public string PropertyName { get; set; }
        public CreateIndexOptions IndexOption { get; set; }
    }
}
