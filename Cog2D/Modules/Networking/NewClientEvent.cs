using Cog.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Networking
{
    public class NewClientEvent : EventParameters
    {
        public CogClient Client { get; private set; }

        public NewClientEvent(Object sender, CogClient client)
            : base(sender)
        {
            this.Client = client;
        }
    }
}
