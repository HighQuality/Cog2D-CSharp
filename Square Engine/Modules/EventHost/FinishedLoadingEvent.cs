using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.EventHost
{
    class FinishedLoadingEvent : EventParameters
    {
        public FinishedLoadingEvent(Object sender)
            : base(sender)
        {
        }
    }
}
