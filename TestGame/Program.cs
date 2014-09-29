using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Square;
using Square.SfmlRenderer;
using Square.Modules.Renderer;
using Square.Modules.EventHost;
using Square.Modules.Content;
using System.Diagnostics;

namespace TestGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine.Initialize<SfmlRenderer>();
            
            Engine.EventHost.RegisterEvent<InitializeEvent>(0, e =>
            {
                // Create and push the initial scene
                Engine.SceneHost.Push(new GameScene());
            });

            Engine.EventHost.RegisterEvent<UpdateEvent>(1, e =>
            {

            });

            Engine.EventHost.RegisterEvent<DrawEvent>(1, e =>
            {
                
            });

            Engine.StartGame("Test Game", 640, 480, WindowStyle.Default);
        }
    }
}
