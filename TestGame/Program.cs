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

            GameObject gameObject = null;

            Engine.EventHost.RegisterEvent<LoadContentEvent>(1, e =>
            {
                var menuScene = Engine.SceneHost.CurrentScene;
                var texture = Engine.Renderer.LoadTexture("texture.png");

                Stopwatch watch = Stopwatch.StartNew();

                for (int i = 0; i < 5; i++)
                {
                    gameObject = new GameObject(menuScene);
                    gameObject.WorldCoord = new Vector2(32f + i * 32f, 32f + i * 32f);
                    gameObject.AddComponenet<MovementComponent>();
                    var spriteComponent = gameObject.AddComponenet<SpriteComponent>();
                    spriteComponent.Texture = texture;
                    spriteComponent.CoordOffset = spriteComponent.Texture.Size / 2f;
                    Console.WriteLine("Object #{0}: {1}ms", i + 1, watch.Elapsed.TotalMilliseconds);

                    watch.Restart();
                }
            });

            Engine.EventHost.RegisterEvent<UpdateEvent>(1, e =>
            {

            });
                        
            Engine.StartGame("Test Game", 640, 480, WindowStyle.Default);
        }
    }
}
