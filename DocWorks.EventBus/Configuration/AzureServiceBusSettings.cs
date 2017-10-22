using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.BuildingBlocks.EventBus.Configuration
{
    public class AzureServiceBusSettings
    {
        public string ServiceBusConnectionString { get; set; }
        public string TopicName { get; set; }
        public string SubscriptionName { get; set; }

        public int MaxConcurrentCalls { get; set; }
    }
}
