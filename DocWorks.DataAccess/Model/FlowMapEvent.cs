using DocWorks.BuildingBlocks.Global.Enumerations;
using System.Dynamic;

namespace DocWorks.Core.Common.Messaging
{
    public class FlowMapEvent
    {
        public SedaService To { get; set; }
        public ApiOperation ApiOperation { get; set; }
        public int Index { get; set; }
        public CMSEventStatus Status { get; set; }

        // TODO: Keep the API operation response here. 2 purposes:
        // May be use this to transfer data to UI (for large payloads)
        // Debugging purpose, to know the output of each API call 
        //public ExpandoObject ApiOperationResponse { get; set; }
    }
}
