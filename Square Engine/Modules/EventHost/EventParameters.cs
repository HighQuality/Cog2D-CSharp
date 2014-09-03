using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.EventHost
{
    public class EventParameters
    {
        public Object Sender { get; private set; }
        public bool Intercept;

        public EventParameters(Object sender)
        {
            this.Sender = sender;
        }
    }
}
