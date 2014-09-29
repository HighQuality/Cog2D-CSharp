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
    public class Client
    {
        private TcpClient client;
        private NetworkStream stream;
        internal BinaryWriter Writer;
        internal BinaryReader Reader;
        private Thread thread;
        private Queue<byte[]> messages = new Queue<byte[]>();
        public bool IsDisconnected { get; private set; }

        internal Client(TcpClient client)
        {
            this.client = client;
            client.NoDelay = true;
            stream = client.GetStream();
            Writer = new BinaryWriter(stream, Encoding.UTF8);
            Reader = new BinaryReader(stream, Encoding.UTF8);
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
            }
        }

        public void Disconnect()
        {
            IsDisconnected = true;
            client.Close();
        }
    }
}
