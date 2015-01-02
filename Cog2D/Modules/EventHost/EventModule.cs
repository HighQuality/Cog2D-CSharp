using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.EventHost
{
    public class EventModule
    {
        public const int DefaultUniqueIdentifier = Int32.MaxValue / 3 * 2;
        private Dictionary<EventIdentifier, IEvent> events = new Dictionary<EventIdentifier, IEvent>();
        
        public EventModule()
        {
        }

        public EventListener<T> RegisterEvent<T>(Object uniqueIdentifier, int priority, Action<T> action)
            where T : EventParameters
        {
            return GetEvent<T>(uniqueIdentifier).Register(priority, action);
        }

        public EventListener<T> RegisterEvent<T>(int priority, Action<T> action)
            where T : EventParameters
        {
            return RegisterEvent<T>(null, priority, action);
        }

        internal IEvent GetEvent(Type type, Object uniqueIdentifier)
        {
            if (!typeof(EventParameters).IsAssignableFrom(type))
                throw new Exception("Type \"" + type.FullName + "\" is not an EventParameters!");
            EventIdentifier identifier;
            identifier.Type = type;
            identifier.UniqueIdentifier = uniqueIdentifier == null ? DefaultUniqueIdentifier : uniqueIdentifier.GetHashCode();

            IEvent eventObject;
            if (events.TryGetValue(identifier, out eventObject))
                return eventObject;
            return null;
        }

        public Event<T> GetEvent<T>(Object uniqueIdentifier)
            where T : EventParameters
        {
            EventIdentifier identifier;
            identifier.Type = typeof(T);
            identifier.UniqueIdentifier = uniqueIdentifier == null ? DefaultUniqueIdentifier : uniqueIdentifier.GetHashCode();

            IEvent eventObject;
            if (!events.TryGetValue(identifier, out eventObject))
            {
                eventObject = new Event<T>(this, identifier);
                events.Add(identifier, eventObject);
            }
            else
            {
                if (!(eventObject is Event<T>))
                    throw new ArgumentException("Template \"" + typeof(T).FullName + "\" did not match " + eventObject.GetType().GetGenericArguments()[0].FullName);
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
