using Cog.Modules.EventHost;
using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public class SpriteComponent
    {
        public GameObject GameObject;
        public ITexture Texture;
        public Vector2 Origin,
            Scale;
        public Color Color = Color.White;

        public static SpriteComponent RegisterOn(GameObject gameObject, ITexture texture)
        {
            var c = new SpriteComponent(gameObject);
            if (texture != null)
            {
                c.Texture = texture;
                c.Origin = texture.Size / 2f;
                c.Origin = new Vector2((int)c.Origin.X, (int)c.Origin.Y);
            }
            gameObject.OnDraw += c.Draw;
            return c;
        }

        public SpriteComponent(GameObject gameObject)
        {
            this.GameObject = gameObject;
        }

        public void Draw(DrawEvent ev, DrawTransformation transformation)
        {
            ev.RenderTarget.RenderTexture(Texture, transformation.WorldCoord, Color, transformation.WorldScale, Origin, transformation.WorldRotation.Degree, new Rectangle(Vector2.Zero, Texture.Size));
        }
    }
}
