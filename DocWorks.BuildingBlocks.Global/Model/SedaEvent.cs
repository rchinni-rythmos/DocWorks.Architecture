using DocWorks.BuildingBlocks.Global.Enumerations;
using DocWorks.BuildingBlocks.Global.Enumerations.Events;
using DocWorks.BuildingBlocks.Global.Model.Events;
using System.Dynamic;

namespace DocWorks.BuildingBlocks.Global.Model
{
    public class SedaEvent
    {
        public SedaEvent(
            string responseId, 
            EventType eventType, 
            BasePayLoad payLoad, 
            EventName eventName = EventName.OrchestratorInit, 
            int eventIndexInFlowMap = 0)
        {
            this.ResponseId = responseId;
            this.EventType = eventType;
            this.PayLoad = payLoad;
            this.EventName = eventName;
            this.EventIndexInFlowMap = eventIndexInFlowMap;
        }
        public EventName EventName { get; set; }
        public string ResponseId { get; set; }
        public EventType EventType { get; set; }
        public BasePayLoad PayLoad { get; set; }
        public int EventIndexInFlowMap { get; set; }
    }
}
