using Square.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.SquareEventHost
{
    /*public class EventHost : IEventHostModule
    {
        public Dictionary<string, IEvent> events = new Dictionary<string, IEvent>();

        public IEvent CreateEvent<T>(string name)
            where T : EventParameters
        {
            IEvent newEvent = new SquareEvent<T>();
            events.Add(name, newEvent);
            return newEvent;
        }
        
        public IEvent CreateEvent(string name)
        {
            return CreateEvent<EventParameters>(name);
        }

        public IEvent FindEvent(string name)
        {
            IEvent eventInstance;
            events.TryGetValue(name, out eventInstance);
            return eventInstance;
        }
    }*/
}
