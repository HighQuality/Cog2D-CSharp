using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Renderer
{
    public interface IRenderTarget
    {
        void SetTransformation(Vector2 center, Vector2 scale, Angle angle);
        void RenderTexture(Texture texture, Vector2 windowCoords);
        void RenderTexture(Texture texture, Vector2 windowCoords, Color color, Vector2 scale, Vector2 origin, float rotation, Rectangle textureRect);

        void Clear(Color color);
    }
}
