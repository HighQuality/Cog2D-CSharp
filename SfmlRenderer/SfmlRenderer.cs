using Cog.Modules.EventHost;
using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.SfmlRenderer
{
    public class SfmlRenderer : RenderModule
    {
        internal static SFML.Graphics.RenderStates RenderState { get; set; }

        public override Window CreateWindow(string title, int width, int height, WindowStyle style)
        {
            return new SfmlWindow(title, width, height, style);
        }

        public override RenderTexture CreateRenderTexture(int width, int height)
        {
            return new SfmlRenderTexture(width, height);
        }

        public override Texture LoadTexture(string filename)
        {
            return new SfmlTexture(filename);
        }

        public override Texture LoadTexture(byte[] data)
        {
            return new SfmlTexture(data);
        }

        public override Texture TextureFromImage(Image image)
        {
            return new SfmlTexture(image);
        }

        protected override void InitializeBlendModes()
        {
            AlphaBlend = new AlphaBlending();
            AdditiveBlend = new AdditiveBlending();
        }
    }
}
