using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.EventHost
{
    public class EventListener<T> : IEventListener
        where T : EventParameters
    {
        private Event<T> eventHost;
        public int Priority { get; private set; }
        public Action<T> Action { get; private set; }
        public bool IsCancelled { get; private set; }

        public EventListener(Event<T> eventHost, Action<T> action)
        {
            this.eventHost = eventHost;
            this.Action = action;
        }

        public void Cancel()
        {
            IsCancelled = true;
        }

        public void SetPriority(int value)
        {
            List<EventListener<T>> list = eventHost.Listeners[-Priority];
            list.Remove(this);
            if (list.Count == 0)
                eventHost.Listeners.Remove(-Priority);

            Priority = value;

            List<EventListener<T>> newList;
            if (!eventHost.Listeners.TryGetValue(-Priority, out newList))
            {
                if (list.Count == 0)
                    newList = list;
                else
                    newList = new List<EventListener<T>>();
                eventHost.Listeners.Add(-Priority, newList);
            }

            newList.Add(this);
        }
    }
}
