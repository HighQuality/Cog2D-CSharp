using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cog.SfmlRenderer
{
    public class SfmlRenderTexture : RenderTexture
    {
        internal SFML.Graphics.RenderTexture InnerRenderTexture;
        private Texture _texture;
        public override Texture Texture { get { return _texture; } }
        public override Vector2 Size { get { return _texture.Size; } }

        public SfmlRenderTexture(int width, int height)
        {
            InnerRenderTexture = new SFML.Graphics.RenderTexture((uint)width, (uint)height, false);
            _texture = new SfmlTexture(this);
        }

        public override void Dispose(bool disposing)
        {
            Texture.Dispose();
            InnerRenderTexture.Dispose();
        }

        public override void Clear(Color color)
        {
            InnerRenderTexture.Clear(new SFML.Graphics.Color((byte)color.R, (byte)color.G, (byte)color.B, (byte)color.A));
            InnerRenderTexture.Display();
        }

        public override void DrawTexture(Texture texture, Vector2 windowCoords, Color color, Vector2 scale, Vector2 origin, float rotation, Rectangle textureRect)
        {
            DrawHelper.DrawTexture(InnerRenderTexture, texture, windowCoords, color, scale, origin, rotation, textureRect);
            InnerRenderTexture.Display();
        }

        public override void DrawTexture(Texture texture, Vector2 windowCoords)
        {
            DrawHelper.DrawTexture(InnerRenderTexture, texture, windowCoords);
            InnerRenderTexture.Display();
        }

        public override void SetTransformation(Vector2 center, Vector2 scale, Angle angle)
        {
            SFML.Graphics.View view = new SFML.Graphics.View();
            view.Center = new SFML.System.Vector2f(center.X, center.Y);
            var size = Size / scale;
            view.Size = new SFML.System.Vector2f(size.X, size.Y);
            view.Rotation = angle.Degree;
            InnerRenderTexture.SetView(view);
        }
    }
}
