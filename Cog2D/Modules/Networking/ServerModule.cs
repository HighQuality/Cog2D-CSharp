using Cog.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cog.Modules.Networking
{
    public class ServerModule
    {
        private Thread thread;
        private TcpListener listener;
        private List<CogClient> clients;
        private Queue<TcpClient> queuedClients = new Queue<TcpClient>();
        private IEventListener updateListener;
        public int Port { get; private set; }
        private Func<TcpClient, CogClient> createClient;

        public ServerModule(int port, Func<TcpClient, CogClient> createClient)
        {
            this.Port = port;
            this.createClient = createClient;

            listener = new TcpListener(System.Net.IPAddress.Any, port);
            try
            {
                listener.Start();

                clients = new List<CogClient>();

                // Start listening for connections
                thread = new Thread(Listener);
                thread.IsBackground = true;
                thread.Start();
                Debug.Success("Listening for connections on {0}...", port);

                updateListener = Engine.EventHost.RegisterEvent<UpdateEvent>(int.MaxValue - 1, Update);
            }
            catch (SocketException e)
            {
                Debug.Error("Could not listen on port {0}: {1}", port, e.Message);
            }
        }

        private void Update(UpdateEvent args)
        {
            lock (queuedClients)
            {
                while (queuedClients.Count > 0)
                {
                    var client = createClient(queuedClients.Dequeue());
                    Debug.Info("{0} Connected!", client.IpAddress);
                    clients.Add(client);
                    Engine.EventHost.GetEvent<NewClientEvent>().Trigger(new NewClientEvent(this, client));
                }
            }

            for (int i = clients.Count - 1; i >= 0; i--)
            {
                clients[i].DispatchMessages();

                if (clients[i].IsDisconnected)
                {
                    clients[i].OnDisconnected();
                    Debug.Event(clients[i].IpAddress + " disconnected!");
                    clients.RemoveAt(i);
                    continue;
                }
            }
        }

        public void Send<T>(T message)
            where T : NetworkMessage
        {
            lock (clients)
                for (int i = clients.Count - 1; i >= 0; i--)
                    clients[i].Send<T>(message);
        }

        public void Send<T>(T message, CogClient except)
            where T : NetworkMessage
        {
            lock (clients)
                for (int i = clients.Count - 1; i >= 0; i--)
                    if (clients[i] != except)
                        clients[i].Send<T>(message);
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
                        lock (queuedClients)
                            queuedClients.Enqueue(tcpClient);
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
