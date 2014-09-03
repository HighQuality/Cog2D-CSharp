using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.EventHost
{
    public class Event<T> : IEvent
        where T : EventParameters
    {
        private SortedList<int, Action<T>> actions = new SortedList<int, Action<T>>();

        public void Trigger(T e)
        {
            foreach (var v in actions)
            {
                v.Value(e);

                if (e.Intercept)
                    break;
            }
        }

        public void Register(int priority, Action<T> action)
        {
            actions.Add(-priority, action);
        }
    }
}
