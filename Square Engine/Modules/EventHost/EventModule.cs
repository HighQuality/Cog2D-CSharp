using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.EventHost
{
    public class EventModule
    {
        private Dictionary<EventIdentifier, IEvent> events = new Dictionary<EventIdentifier, IEvent>();

        public EventModule()
        {
        }

        public EventListener<T> RegisterEvent<T>(object uniqueIdentifier, int priority, Action<T> action)
            where T : EventParameters
        {
            return GetEvent<T>(uniqueIdentifier).Register(priority, action);
        }

        public EventListener<T> RegisterEvent<T>(int priority, Action<T> action)
            where T : EventParameters
        {
            return RegisterEvent<T>(null, priority, action);
        }

        public Event<T> GetEvent<T>(Object uniqueIdentifier)
            where T : EventParameters
        {
            EventIdentifier identifier;
            identifier.Type = typeof(T);
            identifier.UniqueIdentifier = uniqueIdentifier == null ? "".GetHashCode() : uniqueIdentifier.GetHashCode();

            IEvent eventObject;
            if (!events.TryGetValue(identifier, out eventObject))
            {
                eventObject = new Event<T>();
                events.Add(identifier, eventObject);
            }
            else
            {
                if (!(eventObject is Event<T>))
                    throw new ArgumentException("Template \"" + typeof(T).FullName + "\" did not match " + eventObject.GetType().GenericTypeArguments[0].FullName);
            }
            return (Event<T>)eventObject;
        }

        public Event<T> GetEvent<T>()
            where T : EventParameters
        {
            return GetEvent<T>(null);
        }
    }
}
