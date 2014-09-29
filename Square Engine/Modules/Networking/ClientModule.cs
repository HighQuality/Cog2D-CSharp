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
        Client client;
        private IEventListener updateListener;

        internal ClientModule(string hostname, int port)
        {
            var tcpClient = new TcpClient();
            tcpClient.Connect(hostname, port);

            client = new Client(tcpClient);
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
                        ushort messageType = reader.ReadUInt16();
                        NetworkEvent parameters = NetworkEvent.ReadEvent(messageType, reader);

                        var ev = Engine.EventHost.GetEvent(parameters.GetType(), null);
                        if (ev != null)
                        {
                            if (ev.Count > 0)
                                ev.GenericTrigger(parameters);
                            else
                                Console.WriteLine(parameters.GetType().FullName + " has no handler!");
                        }
                        else
                            Console.WriteLine(parameters.GetType().FullName + " has no handler!");
                    }
                }
            }
        }

        public void Raise<T>(T data)
            where T : NetworkEvent
        {
            var buffer = NetworkEvent.ToByteArray<T>(data);
            client.Writer.Write(buffer);
        }

        public void Disconnect()
        {
            client.Disconnect();
        }
    }
}
