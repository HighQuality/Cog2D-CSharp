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
using Cog.Interface;
using System.IO;

namespace TestGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine.Initialize<SfmlRenderer>(new Cog.Image("splash.png"));
            float time = 0f;

            var container = Engine.ResourceHost.Load("main", "resources.crc");

            GameScene scene = null;
            
            Engine.EventHost.RegisterEvent<InitializeEvent>(0, e =>
            {
                /*var message = Engine.ConnectServer("127.0.0.1", 1234);
                if (message != null)
                    Debug.Error(message);
                else
                    Debug.Success("Successfully connected to server @{0}:{1}!", Engine.ClientModule.Hostname, Engine.ClientModule.Port);*/

                // Create and push the initial scene
                scene = new GameScene();
                Engine.SceneHost.Push(scene);

                scene.CreateObject<TestObject>(new Vector2(0f, 0f));
            });
            
            Engine.EventHost.RegisterEvent<UpdateEvent>(1, e =>
            {
                time += e.DeltaTime;
                scene.Camera.LocalCoord = new Vector2(Mathf.Sin(time) * 800f, 0f);
            });
            
            Engine.EventHost.RegisterEvent<DrawEvent>(1, e =>
            {
            });

            Engine.EventHost.RegisterEvent<ExitEvent>(0, e =>
            {
            });
            
            Engine.StartGame("Test Game", WindowStyle.Default);
        }
    }
}
