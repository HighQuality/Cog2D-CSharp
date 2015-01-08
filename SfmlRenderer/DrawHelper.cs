using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cog.SfmlRenderer
{
    static class DrawHelper
    {
        public static void DrawTexture(SFML.Graphics.RenderTarget renderTarget, Texture texture, Vector2 windowCoords, Color color, Vector2 scale, Vector2 origin, float rotation, Rectangle textureRect)
        {
            var sprite = new SFML.Graphics.Sprite();
            sprite.Position = new SFML.System.Vector2f(windowCoords.X, windowCoords.Y);
            sprite.Color = new SFML.Graphics.Color((byte)color.R, (byte)color.G, (byte)color.B, (byte)color.A);
            sprite.Origin = new SFML.System.Vector2f(origin.X, origin.Y);
            sprite.Scale = new SFML.System.Vector2f(scale.X, scale.Y);
            sprite.Texture = ((SfmlTexture)texture).Texture;
            sprite.Rotation = rotation;
            sprite.TextureRect = new SFML.Graphics.IntRect((int)textureRect.TopLeft.X, (int)textureRect.TopLeft.Y, (int)textureRect.Size.X, (int)textureRect.Size.Y);
            renderTarget.Draw(sprite, SfmlRenderer.RenderState);
        }

        public static void DrawTexture(SFML.Graphics.RenderTarget renderTarget, Texture texture, Vector2 windowCoords)
        {
            var sprite = new SFML.Graphics.Sprite();
            sprite.Position = new SFML.System.Vector2f(windowCoords.X, windowCoords.Y);
            sprite.Texture = ((SfmlTexture)texture).Texture;
            sprite.Scale = new SFML.System.Vector2f(1f, 1f);
            renderTarget.Draw(sprite, SfmlRenderer.RenderState);
        }
    }
}
