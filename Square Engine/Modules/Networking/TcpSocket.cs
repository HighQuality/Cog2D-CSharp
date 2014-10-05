using Square.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Square.Modules.Networking
{
    public class TcpSocket : IStringCacher
    {
        private TcpClient client;
        private NetworkStream stream;
        internal BinaryWriter Writer;
        internal BinaryReader Reader;
        private Thread thread;
        private Queue<NetworkMessage> messages = new Queue<NetworkMessage>();
        public bool IsDisconnected { get; private set; }
        public string IpAddress { get; private set; }
        internal List<string> CachedStrings = new List<string>();
        internal Dictionary<string, ushort> CachedStringDictionary = new Dictionary<string, ushort>();
        private Dictionary<string, ushort> sentStrings = new Dictionary<string, ushort>();
        public Permissions Permissions;

        internal TcpSocket(TcpClient client, Permissions permissions)
        {
            this.Permissions = permissions;
            this.client = client;
            IpAddress = client.Client.RemoteEndPoint.ToString();
            client.NoDelay = true;
            stream = client.GetStream();
            Writer = new BinaryWriter(stream, Encoding.UTF8);
            Reader = new BinaryReader(stream, Encoding.UTF8);

            Writer.Write((Int32)Engine.VersionNumber);
            Writer.Write(NetworkMessage.NetworkingHash);

            var remoteVersion = Reader.ReadInt32();
            if (remoteVersion != Engine.VersionNumber)
                throw new InvalidVersionException(string.Format("The Client Version ({0}) did not match our version ({1})", remoteVersion, Engine.VersionNumber));
            byte[] remoteHash = Reader.ReadBytes(NetworkMessage.NetworkingHash.Length);
            for (int i = 0; i < remoteHash.Length; i++)
                if (remoteHash[i] != NetworkMessage.NetworkingHash[i])
                    throw new NetworkHashMismatchException("The networking hash of the client did not match ours!");

            thread = new Thread(Receive);
            thread.IsBackground = true;
            thread.Start();
        }
        
        public void DispatchMessages()
        {
            while (messages.Count > 0)
            {
                messages.Dequeue().Received();
            }
        }
        
        public void Send<T>(T message)
            where T : NetworkMessage
        {
            NetworkMessage.WriteToSocket<T>(message, this);
        }

        private void Receive()
        {
            try
            {
                for (; ; )
                {
                    NetworkMessage receivedMessage = NetworkMessage.ReadMessage(this, this);

                    var properties = receivedMessage.GetType().GetCustomAttribute<MessageExecutionAttribute>();
                    if (properties != null && properties.Immediate)
                        receivedMessage.Received();
                    else
                        lock(messages)
                            messages.Enqueue(receivedMessage);
                }
            }
            catch(IOException e)
            {
                Debug.Error("Stopped Listening: {0}", e.Message);
                Disconnect();
            }
        }

        public ushort GetIdFromString(string value)
        {
            ushort id;
            if (!sentStrings.TryGetValue(value, out id))
            {
                id = (ushort)sentStrings.Count;
                sentStrings.Add(value, id);
                Send(new StringCacheMessage(value));
            }
            return id;
        }

        public string GetStringFromId(ushort id)
        {
            return CachedStrings[(int)id];
        }

        public void Disconnect()
        {
            IsDisconnected = true;
            client.Close();
        }
    }
}
