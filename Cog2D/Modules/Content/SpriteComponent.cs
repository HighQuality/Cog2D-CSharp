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
        public Texture Texture;
        public Vector2 Origin,
            Scale = Vector2.One;
        public Color Color = Color.White;
        public Rectangle TextureRect;

        public static SpriteComponent RegisterOn(GameObject gameObject, Texture texture)
        {
            var c = new SpriteComponent(gameObject);
            if (texture != null)
            {
                c.Texture = texture;
                c.TextureRect = new Rectangle(Vector2.Zero, texture.Size);
                c.Origin = texture.Size / 2f;
                c.Origin = new Vector2((int)c.Origin.X, (int)c.Origin.Y);
            }

            if (gameObject.OnDraw == null)
                gameObject.OnDraw = new List<Action<DrawEvent, DrawTransformation>>();
            gameObject.OnDraw.Add(c.Draw);

            return c;
        }

        private SpriteComponent(GameObject gameObject)
        {
            this.GameObject = gameObject;
        }

        public void Draw(DrawEvent ev, DrawTransformation transformation)
        {
            ev.RenderTarget.RenderTexture(Texture, transformation.WorldCoord, Color, transformation.WorldScale * Scale, Origin, transformation.WorldRotation.Degree, TextureRect);
        }
    }
}
