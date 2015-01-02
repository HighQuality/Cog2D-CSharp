using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.EventHost
{
    public class FinishedLoadingEvent : EventParameters
    {
        public FinishedLoadingEvent(Object sender)
            : base(sender)
        {
        }
    }
}
