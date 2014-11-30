using Cog.Modules.EventHost;
using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public class LabelComponent
    {
        public GameObject GameObject;
        public BitmapFont Font;
        public string Text;
        public Color Color = Color.Black;
        public Vector2 RelativePosition;

        public static LabelComponent RegisterOn(GameObject gameObject, BitmapFont font)
        {
            var c = new LabelComponent(gameObject);
            c.Font = font;
            if (gameObject.OnDraw == null)
                gameObject.OnDraw = new List<Action<DrawEvent, DrawTransformation>>();
            gameObject.OnDraw.Add(c.Draw);

            return c;
        }

        private LabelComponent(GameObject gameObject)
        {
            this.GameObject = gameObject;
        }

        public void Draw(DrawEvent ev, DrawTransformation transformation)
        {
            Font.DrawString(ev.RenderTarget, Text, Color, transformation.WorldCoord + RelativePosition);
        }
    }
}
