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
        public float FontSize;
        public Color Color = Color.Black;
        public Vector2 RelativePosition;
        public HAlign HorizontalAlignment = HAlign.Left;
        public VAlign VerticalAlignment = VAlign.Top;
        public bool HasShadow;

        public static LabelComponent RegisterOn(GameObject gameObject, BitmapFont font, float fontSize)
        {
            var c = new LabelComponent(gameObject);
            c.Font = font;
            c.FontSize = fontSize;
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
            if (HasShadow)
                Font.DrawString(ev.RenderTarget, Text, FontSize, Color.Black, transformation.WorldCoord + RelativePosition + new Vector2(1f, 1f), HorizontalAlignment, VerticalAlignment);
            Font.DrawString(ev.RenderTarget, Text, FontSize, Color, transformation.WorldCoord + RelativePosition, HorizontalAlignment, VerticalAlignment);
        }
    }
}
