using Square.Modules.EventHost;
using Square.Modules.Renderer;
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
            Square.Engine.Initialize<DefaultRenderer>(null);
            
            Square.Engine.EventHost.RegisterEvent<InitializeEvent>(0, e =>
            {
                Square.Engine.SceneHost.Push(new GameScene());
            });

            Square.Engine.StartServer(1234);
        }
    }
}
