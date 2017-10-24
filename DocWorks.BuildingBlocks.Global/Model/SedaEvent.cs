using DocWorks.BuildingBlocks.Global.Enumerations;
using DocWorks.BuildingBlocks.Global.Enumerations.Events;
using DocWorks.BuildingBlocks.Global.Model.Events;

namespace DocWorks.BuildingBlocks.Global.Model
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
