using Cog.Modules.Content;
using Cog.Modules.Networking;
using Cog.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules
{
    public class CogClient : TcpSocket
    {
        internal CogClient(TcpClient client)
            : base(client, Permissions.DefaultClientPermissions)
        {

        }

        public void SubscribeTo(Scene scene)
        {
            scene.AddSubscription(this);
            Send(scene.CreateSceneCreationMessage());
        }

        public bool IsKeyDown(Keyboard.Key key)
        {
            return false;
        }
    }
}
