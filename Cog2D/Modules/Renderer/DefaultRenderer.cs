using Cog.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Renderer
{
    public class DefaultRenderer : RenderModule
    {
        public override Window CreateWindow(string title, int width, int height, WindowStyle style)
        {
            return null;
        }

        public override RenderTexture CreateRenderTexture(int width, int height)
        {
            return null;
        }

        public override Texture LoadTexture(string filename)
        {
            return null;
        }

        public override Texture LoadTexture(byte[] data)
        {
            return null;
        }

        public override Texture TextureFromImage(Image image)
        {
            return null;
        }

        protected override void InitializeBlendModes()
        {
            AlphaBlend = new DefaultBlendMode();
            AdditiveBlend = new DefaultBlendMode();
        }
    }
}
