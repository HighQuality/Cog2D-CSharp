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
        public Action<T> Action { get; private set; }
        public bool IsCancelled { get; private set; }

        public EventListener(Action<T> action)
        {
            this.Action = action;
        }

        public void Cancel()
        {
            IsCancelled = true;
        }
    }
}
