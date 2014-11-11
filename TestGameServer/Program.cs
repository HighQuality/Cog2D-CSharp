using Cog;
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
            Engine.Initialize<DefaultRenderer>(null);

            var container = Engine.ResourceHost.LoadDictionary("main", "resources");
            
            Engine.EventHost.RegisterEvent<InitializeEvent>(0, e =>
            {
                Engine.SceneHost.Push(new GameScene());
            });

            Engine.StartServer(1234);
        }
    }
}
