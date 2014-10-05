using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cog;
using Cog.SfmlRenderer;
using Cog.Modules.Renderer;
using Cog.Modules.EventHost;
using Cog.Modules.Content;
using System.Threading;
using Cog.Modules.Networking;

namespace TestGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine.Initialize<SfmlRenderer>(new Cog.Image("splash.png"));
            
            Engine.EventHost.RegisterEvent<InitializeEvent>(0, e =>
            {
                var message = Engine.ConnectServer("127.0.0.1", 1234);
                if (message != null)
                    Debug.Error(message);
                else
                    Debug.Success("Successfully connected to server @{0}:{1}!", Engine.ClientModule.Hostname, Engine.ClientModule.Port);

                // Create and push the initial scene
                Engine.SceneHost.Push(new GameScene());
            });
            
            Engine.EventHost.RegisterEvent<KeyDownEvent>(Keyboard.Key.Space, 1, e =>
            {
                Engine.ClientModule.Send<TestMessage>(new TestMessage(new Vector2(128f, 64f)));
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
