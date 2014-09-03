using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Square;
using Square.SfmlRenderer;
using Square.Modules.Renderer;
using Square.Modules.EventHost;

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
            });

            Engine.EventHost.RegisterEvent<DrawEvent>(1, e =>
            {
                e.RenderTarget.RenderTexture(texture, new Vector2(0f, 0f));
            });

            Engine.StartGame("Test Game", 640, 480, WindowStyle.Default);
        }
    }
}
