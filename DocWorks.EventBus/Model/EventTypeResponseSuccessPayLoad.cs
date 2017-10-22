using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace DocWorks.BuildingBlocks.EventBus.Model
{
    public class EventTypeResponseSuccessPayLoad : BasePayLoad
    {
        public ExpandoObject Response { get; set; }
    }
}
