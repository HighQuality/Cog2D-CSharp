using Cog;
using Cog.Modules.EventHost;
using Cog.Modules.Networking;
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
            GameScene scene = null;
            
            Engine.EventHost.RegisterEvent<InitializeEvent>(0, e =>
            {
                scene = new GameScene();
                Engine.SceneHost.Push(scene);
            });

            Engine.EventHost.RegisterEvent<NewClientEvent>(0, e =>
            {
                e.Client.SubscribeTo(scene);
            });

            Engine.StartServer(1234);
        }
    }
}
