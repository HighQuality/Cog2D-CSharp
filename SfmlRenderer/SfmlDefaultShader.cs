using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cog.SfmlRenderer
{
    class SfmlDefaultShader : Shader
    {
        public SfmlDefaultShader()
        {

        }

        protected override void Set()
        {
            var rs = SfmlRenderer.RenderState;
            rs.Shader = null;
            SfmlRenderer.RenderState = rs;
        }

        public override void Dispose()
        {
        }
    }
}
