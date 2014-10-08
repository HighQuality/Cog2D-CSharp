using Cog.Modules.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.EventHost
{
    public class EventParameters
    {
        private Object _sender;
        public bool Intercept;

        public Object Sender { get { return _sender; } }

        public EventParameters(Object sender)
        {
            this._sender = sender;
        }
    }
}
