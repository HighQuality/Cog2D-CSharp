using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.Renderer
{
    public interface IRenderTarget
    {
        void RenderTexture(ITexture texture, Vector2 windowCoords);
    }
}
