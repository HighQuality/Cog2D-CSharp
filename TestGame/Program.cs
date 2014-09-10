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

                gameObject = new GameObject(menuScene);
                gameObject.AddComponenet<MovementComponent>();
                var spriteComponent = gameObject.AddComponenet<SpriteComponent>();
                spriteComponent.Texture = Engine.Renderer.LoadTexture("texture.png");
                spriteComponent.CoordOffset = spriteComponent.Texture.Size / 2f;
            });

            Engine.EventHost.RegisterEvent<UpdateEvent>(1, e =>
            {

            });

            Engine.EventHost.RegisterEvent<CloseButtonEvent>(100, e =>
            {
                if (gameObject != null)
                {
                    gameObject.Remove();
                    gameObject = null;

                    e.Intercept = true;
                }
            });
            
            Engine.StartGame("Test Game", 640, 480, WindowStyle.Default);
        }
    }
}
