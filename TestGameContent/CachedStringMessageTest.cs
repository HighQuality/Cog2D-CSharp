using Cog.Modules.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    public class CachedStringMessageTest : NetworkMessage
    {
        [StringProperties(StringSendType.Cached)]
        public string Message;
        bool isResponse = false;

        public CachedStringMessageTest(string message)
        {
            this.Message = message;
        }

        public override void Received()
        {
            Console.WriteLine(Message + " was received!");

            if (!isResponse)
            {
                isResponse = true;
                Sender.Send(this);
            }
        }
    }
}
