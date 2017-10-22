using DocWorks.BuildingBlocks.DataAccess.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace DocWorks.BuildingBlocks.DataAccess.Entity
{
    [CollectionName("FlowMap")]
    public class FlowMap : BaseEntity
    {
        public CMSOperation CMSOperation { get; set; }
        public List<List<int>> Map { get; set; }
        public List<CMSEvent> Events { get; set; }
        public NotificationType NotificationType { get; set; }
        public NotificationTopic NotificationTopic { get; set; }

        public List<CMSEvent> GetNextSetofEvents(int eventIndex)
        {
            var eventList = new List<CMSEvent>();

            if (this.Map.Count >= 0)
            {
                var mapNextEventGroupIndex = 0;

                // First set of events
                if (eventIndex == -1)
                {
                    mapNextEventGroupIndex = 0;
                }
                else
                {
                    // find the location of the event Index
                    for (int i = 0; i < this.Map.Count; i++)
                    {
                        if (this.Map[i].Contains(eventIndex))
                        {
                            // Need the next set of events in the Map
                            mapNextEventGroupIndex = i + 1;
                            break;
                        }
                    }
                }

                // if the event is in the last group, then there is no next set 
                if (mapNextEventGroupIndex < this.Map.Count)
                {
                    var firstIndexes = this.Map[mapNextEventGroupIndex];
                    foreach (var index in firstIndexes)
                    {
                        eventList.Add(this.Events.Find(x => x.Index == index));
                    }
                }
            }

            return eventList;
        }

        /// <summary>
        /// Returns true if all the operations are in status Success
        /// </summary>
        public bool IsOperationComplete { get {
                // if any of the operations are not in success state, then the operation is not complete
                return ! this.Events.Any(x => x.Status != CMSEventStatus.Success);
            }
        }

        /// <summary>
        /// Returns true if all the events in the eventGroup where the event with the given index are complete with status success
        /// </summary>
        /// <param name="eventIndex"></param>
        /// <returns></returns>
        public bool GetCompleteStatusForEventGroupByEventIndex(int eventIndex)
        {
            List<int> eventGroupToCheck = null;
            foreach (var eventGroup in this.Map)
            {
                if(eventGroup.Contains(eventIndex))
                {
                    eventGroupToCheck = eventGroup;
                    break;
                }
            }

            foreach (var ei in eventGroupToCheck)
            {
                var et = this.Events.Find(x => x.Index == ei);

                if (et.Status != CMSEventStatus.Success) return false;
            }

            return true;
        }
    }
}
