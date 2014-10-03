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
        private List<SquareClient> clients;
        private IEventListener updateListener;
        public int Port { get; private set; }

        public ServerModule(int port)
        {
            this.Port = port;

            listener = new TcpListener(System.Net.IPAddress.Any, port);
            try
            {
                listener.Start();

                clients = new List<SquareClient>();

                // Start listening for connections
                thread = new Thread(Listener);
                thread.IsBackground = true;
                thread.Start();
                Debug.Success("Listening for connections on {0}...", port);

                updateListener = Engine.EventHost.RegisterEvent<UpdateEvent>(int.MaxValue, Update);
            }
            catch (SocketException e)
            {
                Debug.Error("Could not listen on port {0}: {1}", port, e.Message);
            }
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
                                UInt16 messageType = reader.ReadUInt16();
                                NetworkMessage receivedMessage = NetworkMessage.ReadEvent(clients[i], messageType, reader);
                                receivedMessage.Received();
                            }
                        }
                    }

                    if (clients[i].IsDisconnected)
                    {
                        Console.WriteLine(clients[i].IpAddress + " disconnected!");
                        clients.RemoveAt(i);
                        continue;
                    }
                }
            }
        }

        public void Send<T>(T data)
            where T : NetworkMessage
        {
            var buffer = NetworkMessage.ToByteArray<T>(data);
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
                    try
                    {
                        var client = new SquareClient(tcpClient);
                        Debug.Info("{0} Connected!", client.IpAddress);

                        lock (clients)
                        {
                            clients.Add(client);
                        }
                    }
                    catch (InvalidVersionException e)
                    {
                        Debug.Error("{0}'s connection failed:\n{1}", tcpClient.Client.RemoteEndPoint, e.Message);
                    }
                    catch (NetworkHashMismatchException e)
                    {
                        Debug.Error("{0}'s connection failed:\n{1}", tcpClient.Client.RemoteEndPoint, e.Message);
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
