using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    internal class TimedEventHost
    {
        private LinkedList<TimedEvent> scheduledEvents;

        public TimedEventHost()
        {
            scheduledEvents = new LinkedList<TimedEvent>();
        }

        public void Schedule(TimedEvent ev)
        {
            var node = scheduledEvents.First;

            if (node == null)
                scheduledEvents.AddFirst(ev);
            else
            {
                for (; ; )
                {
                    // If the event we're currently looking at executes before the new one
                    if (node.Value.Time < ev.Time)
                    {
                        // Look at the next one
                        node = node.Next;

                        // If we're last
                        if (node == null)
                        {
                            scheduledEvents.AddLast(ev);
                            break;
                        }
                    }
                    else
                    {
                        // We're scheduled execute before the current one
                        scheduledEvents.AddBefore(node, ev);
                        break;
                    }
                }
            }
        }


        public void Update()
        {
            while (scheduledEvents.First != null && Engine.TimeStamp >= scheduledEvents.First.Value.Time)
            {
                if (!scheduledEvents.First.Value.IsCancelled)
                    scheduledEvents.First.Value.Action((float)(Engine.TimeStamp - scheduledEvents.First.Value.Time));
                scheduledEvents.RemoveFirst();
            }
        }
    }
}
