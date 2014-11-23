using Cog;
using Cog.Modules.Audio;
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
            Engine.Initialize<DefaultRenderer, DefaultAudioModule>(null);

            var container = Engine.ResourceHost.LoadDictionary("main", "resources");
            GameScene scene = null;
            float x = 16f;
            List<StationaryObject> objects = new List<StationaryObject>();

            float time = 0f;

            Engine.EventHost.RegisterEvent<InitializeEvent>(0, e =>
            {
                scene = Engine.SceneHost.CreateGlobal<GameScene>();
                Engine.SceneHost.Push(scene);
            });

            Engine.EventHost.RegisterEvent<NewClientEvent>(0, e =>
            {
                e.Client.SubscribeTo(scene);
            });

            Engine.EventHost.RegisterEvent<UpdateEvent>(0, e =>
            {
                time += e.DeltaTime;

                while (time >= 1f)
                {
                    objects.Add(scene.CreateObject<StationaryObject>(new Vector2(x, 16f)));

                    while (objects.Count > 6)
                    {
                        objects[0].Remove();
                        objects.RemoveAt(0);
                    }

                    x += 32f;
                    if (x >= 320f)
                        x -= 640f;

                    time -= 0.1f;
                }
            });

            Engine.StartServer(1234);
        }
    }
}
