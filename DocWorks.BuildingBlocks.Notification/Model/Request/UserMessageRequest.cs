using DocWorks.BuildingBlocks.Global.Model.Request;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace DocWorks.BuildingBlocks.Notification.Model.Request
{
    public class UserMessageRequest : BaseRequest
    {
        public MessageContent MessageContent { get; set; }

        //TODO: Time to live to discuss with @Pranav
       // public string time_to_live { get; set; }
    }
}
