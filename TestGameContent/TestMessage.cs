using Cog.Modules.Networking;
using Cog.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    public class TestMessage : NetworkMessage
    {
        public Scene Scene;

        public override void Received()
        {
            if (Scene == null)
                Console.WriteLine("Scene is null");
            else
                Console.WriteLine("Scene is named {0}", Scene.Name);
        }
    }
}
