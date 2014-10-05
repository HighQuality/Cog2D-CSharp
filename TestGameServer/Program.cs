using Cog.Modules.EventHost;
using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestGame;

namespace TestGameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Cog.Engine.Initialize<DefaultRenderer>(null);
            
            Cog.Engine.EventHost.RegisterEvent<InitializeEvent>(0, e =>
            {
                Cog.Engine.SceneHost.Push(new GameScene());
            });

            Cog.Engine.StartServer(1234);
        }
    }
}
