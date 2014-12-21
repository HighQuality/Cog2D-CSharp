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
        protected override void Set(IRenderTarget window)
        {
            if (!(window is SfmlWindow))
                throw new ArgumentException("window must be an SfmlWindow!");
            SfmlWindow w = (SfmlWindow)window;
            w.RenderState = new SFML.Graphics.RenderStates(SFML.Graphics.BlendMode.Alpha);
        }
    }
}
