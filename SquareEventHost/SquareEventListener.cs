using Square.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.SquareEventHost
{
    class SquareEventListener : IEventListener
    {
        public Func<EventParameters, bool> Action;
        public bool IsCancelled { get; private set; }

        public void Cancel()
        {
            IsCancelled = true;
        }
    }
}
