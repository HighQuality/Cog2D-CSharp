using Square;
using Square.Modules.Networking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    public class PingMessage : NetworkMessage
    {
        private static Dictionary<int, Stopwatch> watches = new Dictionary<int,Stopwatch>();
        private static int nextId = 1;

        public int Id;
        public bool IsResponse;

        public PingMessage()
        {
            this.Id = nextId++;
            watches.Add(Id, Stopwatch.StartNew());
        }

        public override void Received()
        {
            if (IsResponse)
            {
                Stopwatch watch;
                if (watches.TryGetValue(Id, out watch))
                {
                    Console.WriteLine("Ping: {0}ms", watch.Elapsed.TotalMilliseconds);
                    watches.Remove(Id);
                }
                else
                    Console.WriteLine("No Ping Message with ID " + Id.ToString() + " expected!");
            }
            else
            {
                // Return message
                IsResponse = true;
                Sender.Send<PingMessage>(this);
            }
        }
    }
}
