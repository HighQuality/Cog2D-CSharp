using Cog;
using Cog.Modules.Content;
using Cog.Modules.EventHost;
using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    public class Dot : GameObject
    {
        private static Texture texture;
        SpriteComponent c;

        public Dot()
        {
            if (texture == null)
            {
                var img = new Image(1, 1);
                img.SetColor(0, 0, Color.Black);
                texture = Engine.Renderer.TextureFromImage(img);
            }

            c = SpriteComponent.RegisterOn(this, texture);
        }
    }
}
