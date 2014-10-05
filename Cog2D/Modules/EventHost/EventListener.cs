using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.EventHost
{
    public class EventListener<T> : IEventListener
        where T : EventParameters
    {
        public IEvent IEvent { get { return Event; } }
        public Event<T> Event { get; private set; }
        public int Priority { get; private set; }
        public Action<T> Action { get; private set; }
        public bool IsCancelled { get; private set; }

        public EventListener(Event<T> eventHost, Action<T> action)
        {
            this.Event = eventHost;
            this.Action = action;
        }

        public void Cancel()
        {
            IsCancelled = true;
        }

        public void SetPriority(int value)
        {
            List<EventListener<T>> list = Event.Listeners[-Priority];
            list.Remove(this);
            if (list.Count == 0)
                Event.Listeners.Remove(-Priority);

            Priority = value;

            List<EventListener<T>> newList;
            if (!Event.Listeners.TryGetValue(-Priority, out newList))
            {
                if (list.Count == 0)
                    newList = list;
                else
                    newList = new List<EventListener<T>>();
                Event.Listeners.Add(-Priority, newList);
            }

            newList.Add(this);
        }
    }
}
