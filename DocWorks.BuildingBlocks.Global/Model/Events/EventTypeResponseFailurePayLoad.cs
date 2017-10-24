using DocWorks.BuildingBlocks.Global.Model.ErrorHandling;

namespace DocWorks.BuildingBlocks.Global.Model.Events
{
    public class EventTypeResponseFailurePayLoad : BasePayLoad
    {
        public ErrorResponse Failure { get; set; }
    }
}
