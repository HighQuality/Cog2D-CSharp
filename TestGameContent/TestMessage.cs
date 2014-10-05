using Square;
using Square.Modules.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    public class TestMessage : NetworkMessage
    {
        Vector2 vector;

        public TestMessage(Vector2 vector)
        {
            this.vector = vector;
        }

        public override void Received()
        {
            Console.WriteLine(vector.ToString());
        }
    }
}
