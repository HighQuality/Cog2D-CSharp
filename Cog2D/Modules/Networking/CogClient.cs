using Cog.Modules.EventHost;
using Cog.Scenes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cog.Modules.Networking
{
    public class CogClient : IStringCacher
    {
        private TcpClient client;
        private NetworkStream stream;
        internal BinaryWriter Writer;
        internal BinaryReader Reader;
        private Thread thread;
        private Queue<MessageData> messages = new Queue<MessageData>();
        public bool IsDisconnected { get; private set; }
        public string IpAddress { get; private set; }
        public string Identifier { get { return IpAddress; } }
        internal List<string> CachedStrings = new List<string>();
        internal Dictionary<string, ushort> CachedStringDictionary = new Dictionary<string, ushort>();
        private Dictionary<string, ushort> sentStrings = new Dictionary<string, ushort>();
        public Permissions Permissions;

        public CogClient(TcpClient client, Permissions permissions)
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
                var msgData = messages.Dequeue();
                var msg = NetworkMessage.ReadMessage(msgData.TypeId, msgData.Data, this);
                msg.Received();
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
                    UInt16 typeId = Reader.ReadUInt16();
                    var type = NetworkMessage.GetType(typeId);
                    var data = NetworkMessage.ReadMessageData(typeId, this, Reader);

                    var properties = (MessageExecutionAttribute)type.GetCustomAttributes(typeof(MessageExecutionAttribute), true).FirstOrDefault();
                    if (properties != null && properties.Immediate)
                    {
                        var msg = NetworkMessage.ReadMessage(typeId, data, this);
                        msg.Received();
                    }
                    else
                    {
                        MessageData msgData;
                        msgData.TypeId = typeId;
                        msgData.Data = data;
                        lock (messages)
                            messages.Enqueue(msgData);
                    }
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

        public void SubscribeTo(Scene scene)
        {
            Send(scene.CreateSceneCreationMessage());
            scene.AddSubscription(this);
        }

        public bool IsKeyDown(Keyboard.Key key)
        {
            return false;
        }
    }
}
