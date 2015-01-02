using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Networking
{
    class ServerConnection : CogClient
    {
        public ServerConnection(TcpClient client)
            : base(client, Permissions.FullPermissions)
        {
        }
    }
}
