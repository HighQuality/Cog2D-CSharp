﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Renderer
{
    public interface IRenderTarget
    {
        void RenderTexture(ITexture texture, Vector2 windowCoords);
        void RenderTexture(ITexture texture, Vector2 windowCoords, Vector2 scale, Vector2 origin, float rotation, Rectangle textureRect);
    }
}
