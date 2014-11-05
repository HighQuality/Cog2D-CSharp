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
            
            TestObject mainObj = null,
                otherObj = null;

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

                var baseObj = scene.CreateObject<SpriteObject>(Vector2.Zero);
                baseObj.Sprite.Texture = Engine.Renderer.LoadTexture("world.png");
                baseObj.Sprite.Origin = new Vector2(300f, 300f);

                mainObj = scene.CreateObject<TestObject>(baseObj, new Vector2(0f, 0f));
                mainObj.Movement.LeftKey = Keyboard.Key.Left;
                mainObj.Movement.RightKey = Keyboard.Key.Right;
                mainObj.Movement.UpKey = Keyboard.Key.Up;
                mainObj.Movement.DownKey = Keyboard.Key.Down;
                mainObj.LocalRotation = new Angle(-45f);

                otherObj = scene.CreateObject<TestObject>(baseObj, new Vector2(0f, 0f));
                otherObj.Movement.LeftKey = Keyboard.Key.A;
                otherObj.Movement.RightKey = Keyboard.Key.D;
                otherObj.Movement.UpKey = Keyboard.Key.W;
                otherObj.Movement.DownKey = Keyboard.Key.S;
            });
            
            Engine.EventHost.RegisterEvent<UpdateEvent>(1, e =>
            {
                scene.Camera.WorldCoord = (mainObj.WorldCoord + otherObj.WorldCoord) / 2f;

                time += e.DeltaTime;
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
