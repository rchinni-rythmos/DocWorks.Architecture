using DocWorks.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.BuildingBlocks.EventBus.Model
{
    public class EventHandlerInput
    {
        public string ResponseId { get; set; }
        public int EventIndexInFlowMap { get; set; }
        public EventTypeRequestPayLoad PayLoad { get; set; }
    }
}
