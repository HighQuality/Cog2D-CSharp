using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.EventHost
{
    public class LoadContentEvent : EventParameters
    {
        public LoadContentEvent(Object sender)
            : base(sender)
        {
        }
    }
}
