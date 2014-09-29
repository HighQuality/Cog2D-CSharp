using Square.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Square.Modules.Networking
{
    public class ServerModule
    {
        private Thread thread;
        private TcpListener listener;
        private List<Client> clients;
        private IEventListener updateListener;

        public ServerModule(int port)
        {
            listener = new TcpListener(System.Net.IPAddress.Any, port);
            listener.Start();

            clients = new List<Client>();
            
            // Start listening for connections
            thread = new Thread(Listener);
            thread.IsBackground = true;
            thread.Start();
            Debug.Info("Listening for connections on {0}...", port);

            updateListener = Engine.EventHost.RegisterEvent<UpdateEvent>(int.MaxValue, Update);
        }

        private void Update(UpdateEvent args)
        {
            lock (clients)
            {
                for (int i = clients.Count - 1; i >= 0; i--)
                {
                    byte[] message;
                    while ((message = clients[i].DequeueMessage()) != null)
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

                    if (clients[i].IsDisconnected)
                    {
                        clients.RemoveAt(i);
                        continue;
                    }
                }
            }
        }

        public void Raise<T>(T data)
            where T : NetworkEvent
        {
            var buffer = NetworkEvent.ToByteArray<T>(data);
            lock (clients)
                for (int i = clients.Count - 1; i >= 0; i--)
                    clients[i].Writer.Write(buffer);
        }

        public void StopServer()
        {
            listener.Stop();
            listener = null;

            updateListener.Cancel();
            updateListener = null;
        }

        private void Listener()
        {
            try
            {
                while (listener.Server.IsBound)
                {
                    var tcpClient = listener.AcceptTcpClient();

                    lock (clients)
                    {
                        clients.Add(new Client(tcpClient));
                    }
                }
            }
            catch (SocketException e)
            {
                Debug.Error("Stopped listening: {0}", e.Message);
            }
        }
    }
}
