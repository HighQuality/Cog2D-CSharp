using Cog.Modules.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cog.Modules.Renderer
{
    public abstract class RenderTexture : IRenderTarget, IDisposable
    {
        public abstract Texture Texture { get; }
        public abstract Vector2 Size { get; }

        public abstract void SetTransformation(Vector2 center, Vector2 scale, Angle angle);
        public abstract void DrawTexture(Texture texture, Vector2 windowCoords);
        public abstract void DrawTexture(Texture texture, Vector2 windowCoords, Color color, Vector2 scale, Vector2 origin, float rotation, Rectangle textureRect);

        public abstract void Clear(Color color);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public abstract void Dispose(bool disposing);

        ~RenderTexture()
        {
            Dispose(false);
        }
    }
}
