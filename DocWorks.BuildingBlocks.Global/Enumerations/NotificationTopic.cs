using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.BuildingBlocks.Global.Enumerations
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum NotificationTopic
    {
        NA=0,
        Project=1
    }
}
