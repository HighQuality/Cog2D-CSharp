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
    public class TcpSocket
    {
        private TcpClient client;
        private NetworkStream stream;
        internal BinaryWriter Writer;
        internal BinaryReader Reader;
        private Thread thread;
        private Queue<byte[]> messages = new Queue<byte[]>();
        public bool IsDisconnected { get; private set; }
        public string IPAddress { get; private set; }

        internal TcpSocket(TcpClient client)
        {
            this.client = client;
            IPAddress = client.Client.RemoteEndPoint.ToString();
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

        /// <summary>
        /// Tries to dequeue a received message, returns null if there currently are no messages queued
        /// </summary>
        public byte[] DequeueMessage()
        {
            lock (messages)
                if (messages.Count > 0)
                    return messages.Dequeue();
            return null;
        }
        
        private void Receive()
        {
            try
            {
                for (; ; )
                {
                    UInt32 messageSize = Reader.ReadUInt32();
                    byte[] messageData = Reader.ReadBytes((int)messageSize);

                    lock (messages)
                        messages.Enqueue(messageData);
                }
            }
            catch(IOException e)
            {
                Debug.Error("Stopped Listening: {0}", e.Message);
                Disconnect();
            }
        }

        public void Disconnect()
        {
            IsDisconnected = true;
            client.Close();
        }

        public void Send<T>(T data)
            where T : NetworkMessage
        {
            var buffer = NetworkMessage.ToByteArray<T>(data);
            Writer.Write(buffer);
        }
    }
}
