﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.EventHost
{
    public class Event<T> : IEvent
        where T : EventParameters
    {
        private SortedList<int, List<EventListener<T>>> listeners = new SortedList<int, List<EventListener<T>>>();
        private Stack<int> toBeRemoved = new Stack<int>();

        public Event()
        {
        }

        /// <summary>
        /// Triggers the event.
        /// </summary>
        /// <param name="e">The Event Parameters</param>
        /// <returns>Returns true if the event was intercepted</returns>
        public bool Trigger(T e)
        {
            bool breakOut = false;
            bool returnValue = false;
            foreach (var listenerList in listeners.Values)
            {
                for (int i = listenerList.Count - 1; i >= 0; i--)
                {
                    var listener = (EventListener<T>)listenerList[i];
                    if (listener.IsCancelled)
                    {
                        listenerList.RemoveAt(i);
                        if (listenerList.Count == 0)
                            toBeRemoved.Push(i);
                    }
                    else
                    {
                        listener.Action(e);

                        if (e.Intercept)
                        {
                            breakOut = true;
                            returnValue = true;
                            break;
                        }
                    }
                }

                if (breakOut)
                    break;
            }

            while (toBeRemoved.Count > 0)
                listeners.Remove(toBeRemoved.Pop());

            return returnValue;
        }

        public EventListener<T> Register(int priority, Action<T> action)
        {
            var listener = new EventListener<T>(action);
            List<EventListener<T>> listenerList;
            if (!listeners.TryGetValue(priority, out listenerList))
            {
                listenerList = new List<EventListener<T>>();
                listeners.Add(-priority, listenerList);
            }
            listenerList.Add(listener);
            return listener;
        }
    }
}
