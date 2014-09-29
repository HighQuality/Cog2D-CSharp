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
using System.Threading;

namespace TestGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine.Initialize<SfmlRenderer>();
            
            Engine.EventHost.RegisterEvent<InitializeEvent>(0, e =>
            {
                var message = Engine.ConnectServer("localhost", 1234);
                if (message != null)
                    Debug.Error(message);
                else
                    Debug.Success("Successfully connected to server!");

                // Create and push the initial scene
                Engine.SceneHost.Push(new GameScene());
            });

            Engine.EventHost.RegisterEvent<KeyDownEvent>(Keyboard.Key.Space, 1, e =>
            {
                System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
                Engine.ClientModule.Raise<LeaveMessage>(new LeaveMessage("Here's a random number: "));
                Console.WriteLine(watch.Elapsed.TotalMilliseconds);
            });

            Engine.EventHost.RegisterEvent<DrawEvent>(1, e =>
            {
                
            });

            Engine.EventHost.RegisterEvent<ExitEvent>(0, e =>
            {
            });

            Engine.StartGame("Test Game", 640, 480, WindowStyle.Default);
        }
    }
}
