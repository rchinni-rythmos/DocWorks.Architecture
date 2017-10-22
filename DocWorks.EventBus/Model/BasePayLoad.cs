using DocWorks.BuildingBlocks.ErrorHandling.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace DocWorks.BuildingBlocks.EventBus.Model
{
    public class BasePayLoad
    {
        public ExpandoObject Request { get; set; }
        
    }
}
