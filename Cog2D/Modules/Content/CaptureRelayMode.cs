using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public enum CaptureRelayMode
    {
        /// <summary>
        /// Relays to the server which relays to all clients who have the object loaded
        /// </summary>
        ServerClientRelay,
        /// <summary>
        /// Relay to only the server
        /// </summary>
        ServerRelay,
        /// <summary>
        /// Don't relay
        /// </summary>
        NoRelay
    }
}
