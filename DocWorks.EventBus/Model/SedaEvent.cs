using DocWorks.BuildingBlocks.ErrorHandling.Model;
using DocWorks.BuildingBlocks.EventBus.Enumerations;
using DocWorks.BuildingBlocks.Global.Enumerations;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace DocWorks.BuildingBlocks.EventBus.Model
{
    public class SedaEvent
    {
        public EventName EventName { get; set; }
        public string ResponseId { get; set; }
        public SedaService From { get; set; }
        public SedaService To { get; set; }
        public EventType EventType { get; set; }
        public Priority Priority { get; set; }
        public BasePayLoad PayLoad { get; set; }
        public int EventIndexInFlowMap { get; set; }
        public CmsOperation CmsOperation { get; set; }
    }

    
}
