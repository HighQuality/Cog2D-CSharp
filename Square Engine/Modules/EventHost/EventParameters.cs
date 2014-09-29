using Square.Modules.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.EventHost
{
    public class EventParameters
    {
        [NetworkIgnore()]
        private Object _sender;
        [NetworkIgnore()]
        public bool Intercept;

        public Object Sender { get { return _sender; } }

        public EventParameters(Object sender)
        {
            this._sender = sender;
        }
    }
}
