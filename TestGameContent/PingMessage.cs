using Cog;
using Cog.Modules.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    public class PingMessage : NetworkMessage
    {
        private static Dictionary<int, System.Diagnostics.Stopwatch> watches = new Dictionary<int, System.Diagnostics.Stopwatch>();
        private static int nextId = 1;

        public int Id;
        public bool IsResponse;

        public PingMessage()
        {
            this.Id = nextId++;
            watches.Add(Id, System.Diagnostics.Stopwatch.StartNew());
        }

        public override void Received()
        {
            if (IsResponse)
            {
                System.Diagnostics.Stopwatch watch;
                if (watches.TryGetValue(Id, out watch))
                {
                    Debug.Info("Ping: {0}ms", watch.Elapsed.TotalMilliseconds);
                    watches.Remove(Id);
                }
                else
                    Debug.Error("No Ping Message with ID " + Id.ToString() + " expected!");
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
