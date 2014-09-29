using Square.Modules.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    public class LeaveMessage : NetworkEvent
    {
        public string Message;
        public int RandomizedNumber;

        public LeaveMessage(string message)
        {
            this.Message = message;
            RandomizedNumber = new Random().Next(1, 101);
        }
    }
}
