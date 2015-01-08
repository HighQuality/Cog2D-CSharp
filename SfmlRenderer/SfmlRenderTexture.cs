using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cog.SfmlRenderer
{
    public class SfmlDrawTexture : RenderTexture
    {
        internal SFML.Graphics.RenderTexture InnerDrawTexture;
        private Texture _texture;
        public override Texture Texture { get { return _texture; } }
        public override Vector2 Size { get { return _texture.Size; } }

        public SfmlDrawTexture(int width, int height)
        {
            InnerDrawTexture = new SFML.Graphics.RenderTexture((uint)width, (uint)height, false);
            _texture = new SfmlTexture(this);
        }

        public override void Dispose(bool disposing)
        {
            Texture.Dispose();
            InnerDrawTexture.Dispose();
        }

        public override void Clear(Color color)
        {
            InnerDrawTexture.Clear(new SFML.Graphics.Color((byte)color.R, (byte)color.G, (byte)color.B, (byte)color.A));
            InnerDrawTexture.Display();
        }

        public override void DrawTexture(Texture texture, Vector2 windowCoords, Color color, Vector2 scale, Vector2 origin, float rotation, Rectangle textureRect)
        {
            DrawHelper.DrawTexture(InnerDrawTexture, texture, windowCoords, color, scale, origin, rotation, textureRect);
            InnerDrawTexture.Display();
        }

        public override void DrawTexture(Texture texture, Vector2 windowCoords)
        {
            DrawHelper.DrawTexture(InnerDrawTexture, texture, windowCoords);
            InnerDrawTexture.Display();
        }

        public override void SetTransformation(Vector2 center, Vector2 scale, Angle angle)
        {
            SFML.Graphics.View view = new SFML.Graphics.View();
            view.Center = new SFML.System.Vector2f(center.X, center.Y);
            var size = Size / scale;
            view.Size = new SFML.System.Vector2f(size.X, size.Y);
            view.Rotation = angle.Degree;
            InnerDrawTexture.SetView(view);
        }
    }
}
