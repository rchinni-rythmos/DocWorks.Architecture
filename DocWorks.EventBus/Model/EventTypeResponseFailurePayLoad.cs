using DocWorks.BuildingBlocks.ErrorHandling.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.BuildingBlocks.EventBus.Model
{
    public class EventTypeResponseFailurePayLoad : BasePayLoad
    {
        public ErrorResponse Failure { get; set; }
    }
}
