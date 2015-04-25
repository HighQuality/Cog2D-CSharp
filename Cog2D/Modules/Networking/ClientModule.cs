using Cog.Modules.EventHost;
using Cog.Scenes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Networking
{
    public class ClientModule
    {
        ServerConnection client;
        private IEventListener updateListener;
        public string Hostname { get; private set; }
        public int Port { get; private set; }

        public List<Scene> RemotelyCreatedScenes = new List<Scene>();

        internal ClientModule(string hostname, int port)
        {
            this.Hostname = hostname;
            this.Port = port;
            var tcpClient = new TcpClient();
            tcpClient.Connect(hostname, port);
            
            client = new ServerConnection(tcpClient);
            updateListener = Engine.EventHost.RegisterEvent<UpdateEvent>(int.MaxValue - 1, Update);
        }

        private void Update(UpdateEvent args)
        {
            client.DispatchMessages();
        }
        
        public void Send<T>(T message)
            where T : NetworkMessage
        {
            client.Send<T>(message);
        }

        internal void Disconnect()
        {
            foreach (var scene in RemotelyCreatedScenes)
                scene.Remove();
            RemotelyCreatedScenes.Clear();
            client.Disconnect();
        }
    }
}
