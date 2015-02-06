using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.SfmlRenderer
{
    class AdditiveBlending : BlendMode
    {
        protected override void Set()
        {
            var rs = SfmlRenderer.RenderState;
            rs.BlendMode = SFML.Graphics.BlendMode.Add;
            SfmlRenderer.RenderState = rs;
        }
    }
}
