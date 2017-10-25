using DocWorks.BuildingBlocks.Global.Enumerations;
using DocWorks.BuildingBlocks.Global.Enumerations.Events;
using DocWorks.BuildingBlocks.Global.Model.Events;
using System.Dynamic;

namespace DocWorks.BuildingBlocks.Global.Model
{
    public class SedaEvent
    {
        public SedaEvent(string responseId, 
            SedaService toService,
            SedaService fromService, 
            EventType eventType, 
            CmsOperation cmsOperation,
            Priority priority,
            BasePayLoad payLoad, 
            EventName eventName = EventName.Init, 
            int eventIndexInFlowMap = 0)
        {
            this.ResponseId = responseId;
            this.To = toService;
            this.From = From;
            this.EventType = eventType;
            this.CmsOperation = cmsOperation;
            this.Priority = priority;
            this.PayLoad = payLoad;
            this.EventName = eventName;
            this.EventIndexInFlowMap = eventIndexInFlowMap;
        }
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
