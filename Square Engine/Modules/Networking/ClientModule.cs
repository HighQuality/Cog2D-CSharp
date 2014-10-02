using Square.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.Networking
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

            client = new TcpSocket(tcpClient);
            updateListener = Engine.EventHost.RegisterEvent<UpdateEvent>(int.MaxValue, Update);
        }

        private void Update(UpdateEvent args)
        {
            byte[] message;
            while ((message = client.DequeueMessage()) != null)
            {
                using (MemoryStream stream = new MemoryStream(message))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        UInt16 messageType = reader.ReadUInt16();
                        NetworkMessage receivedMessage = NetworkMessage.ReadEvent(client, messageType, reader);
                        receivedMessage.Received();
                    }
                }
            }
        }
        
        public void Raise<T>(T data)
            where T : NetworkMessage
        {
            var buffer = NetworkMessage.ToByteArray<T>(data);
            client.Writer.Write(buffer);
        }

        public void Disconnect()
        {
            client.Disconnect();
        }
    }
}
