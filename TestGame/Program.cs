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

namespace TestGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine.Initialize<SfmlRenderer>(new Cog.Image("splash.png"));
            float time = 0f;

            TestObject mainObj = null;
            
            Engine.EventHost.RegisterEvent<InitializeEvent>(0, e =>
            {
                /*var message = Engine.ConnectServer("127.0.0.1", 1234);
                if (message != null)
                    Debug.Error(message);
                else
                    Debug.Success("Successfully connected to server @{0}:{1}!", Engine.ClientModule.Hostname, Engine.ClientModule.Port);*/

                // Create and push the initial scene
                var scene = new GameScene();
                Engine.SceneHost.Push(scene);

                mainObj = scene.CreateObject<TestObject>(new Vector2(64f, 64f));
                mainObj.Movement.LeftKey = Keyboard.Key.Left;
                mainObj.Movement.RightKey = Keyboard.Key.Right;
                mainObj.Movement.UpKey = Keyboard.Key.Up;
                mainObj.Movement.DownKey = Keyboard.Key.Down;

                var w = scene.CreateObject<TestObject>(mainObj, new Vector2(32f, 32f));
                w.Movement.LeftKey = Keyboard.Key.A;
                w.Movement.RightKey = Keyboard.Key.D;
                w.Movement.UpKey = Keyboard.Key.W;
                w.Movement.DownKey = Keyboard.Key.S;
            });
            
            Engine.EventHost.RegisterEvent<UpdateEvent>(1, e =>
            {
                time += e.DeltaTime;
                mainObj.LocalRotation = new Angle(90f * time);
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
