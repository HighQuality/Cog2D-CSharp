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
                mainObj.Movement.Left = new KeyCapture(mainObj, Keyboard.Key.Left, 0, CaptureRelayMode.NoRelay);
                mainObj.Movement.Right = new KeyCapture(mainObj, Keyboard.Key.Right, 0, CaptureRelayMode.NoRelay);
                mainObj.Movement.Up = new KeyCapture(mainObj, Keyboard.Key.Up, 0, CaptureRelayMode.NoRelay);
                mainObj.Movement.Down = new KeyCapture(mainObj, Keyboard.Key.Down, 0, CaptureRelayMode.NoRelay);
                mainObj.LocalRotation = new Angle(-45f);

                otherObj = scene.CreateObject<TestObject>(baseObj, new Vector2(0f, 0f));
                otherObj.Movement.Left = new KeyCapture(mainObj, Keyboard.Key.A, 0, CaptureRelayMode.NoRelay);
                otherObj.Movement.Right = new KeyCapture(mainObj, Keyboard.Key.D, 0, CaptureRelayMode.NoRelay);
                otherObj.Movement.Up = new KeyCapture(mainObj, Keyboard.Key.W, 0, CaptureRelayMode.NoRelay);
                otherObj.Movement.Down = new KeyCapture(mainObj, Keyboard.Key.S, 0, CaptureRelayMode.NoRelay);
                otherObj.Parent = mainObj;
            });
            
            Engine.EventHost.RegisterEvent<UpdateEvent>(1, e =>
            {
                scene.Camera.WorldCoord = (mainObj.WorldCoord + otherObj.WorldCoord) / 2f;
                scene.Camera.WorldRotation = otherObj.WorldRotation;

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
