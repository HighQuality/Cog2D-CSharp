using Cog.Modules.EventHost;
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
        TcpSocket client;
        private IEventListener updateListener;
        public string Hostname { get; private set; }
        public int Port { get; private set; }

        internal ClientModule(string hostname, int port)
        {
            this.Hostname = hostname;
            this.Port = port;
            var tcpClient = new TcpClient();
            tcpClient.Connect(hostname, port);

            client = new TcpSocket(tcpClient, Permissions.DefaultClientPermissions);
            updateListener = Engine.EventHost.RegisterEvent<UpdateEvent>(int.MaxValue, Update);
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

        public void Disconnect()
        {
            client.Disconnect();
        }
    }
}
