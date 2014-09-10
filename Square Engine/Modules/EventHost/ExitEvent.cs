using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.EventHost
{
    public class ExitEvent : EventParameters
    {
        public ExitEvent(Object sender)
            : base(sender)
        {
        }
    }
}
