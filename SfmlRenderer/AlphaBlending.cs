using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.SfmlRenderer
{
    class AlphaBlending : BlendMode
    {
        protected override void Set()
        {
            var rs = SfmlRenderer.RenderState;
            rs.BlendMode = SFML.Graphics.BlendMode.Alpha;
            SfmlRenderer.RenderState = rs;
        }
    }
}
