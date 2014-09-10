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

            ITexture texture = null;

            Engine.EventHost.RegisterEvent<LoadContentEvent>(1, e =>
            {
                texture = Engine.Renderer.LoadTexture("texture.png");

                var menuScene = Engine.SceneHost.CurrentScene;

                var obj = new GameObject(menuScene);
                obj.AddComponenet<MovementComponent>();
                var spriteComponent = obj.AddComponenet<SpriteComponent>();
                spriteComponent.Texture = texture;
                //spriteComponent.CoordOffset = texture.Size / 2f;
            });

            Engine.EventHost.RegisterEvent<UpdateEvent>(1, e =>
            {

            });
            
            Engine.StartGame("Test Game", 640, 480, WindowStyle.Default);
        }
    }
}
