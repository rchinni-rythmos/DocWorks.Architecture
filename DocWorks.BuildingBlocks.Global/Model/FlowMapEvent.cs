using DocWorks.BuildingBlocks.Global.Enumerations;
using DocWorks.BuildingBlocks.Global.Enumerations.Events;

namespace DocWorks.BuildingBlocks.Global.Model
{
    public class FlowMapEvent
    {
        public SedaService To { get; set; }
        public EventName EventName { get; set; }
        public int Index { get; set; }
        public EventStatus Status { get; set; }

        // TODO: Keep the API operation response here. 2 purposes:
        // May be use this to transfer data to UI (for large payloads)
        // Debugging purpose, to know the output of each API call 
        //public ExpandoObject ApiOperationResponse { get; set; }
    }
}
