using Square.Modules.Content;
using Square.Modules.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules
{
    public class SquareClient : TcpSocket
    {
        internal SquareClient(TcpClient client)
            : base(client, Permissions.DefaultClientPermissions)
        {

        }

        public bool IsKeyDown(Keyboard.Key key)
        {
            return false;
        }
    }
}
