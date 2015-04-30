using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XnaRenderer
{
    public class XnaRenderer : RenderModule
    {
        public override RenderTexture CreateRenderTexture(int width, int height)
        {
            throw new NotImplementedException();
        }

        public override Window CreateWindow(string title, int width, int height, WindowStyle style)
        {
            return new XnaWindow(title, width, height, style);
        }

        public override Texture LoadTexture(byte[] data)
        {
            return new XnaTexture(data);
        }

        public override Texture TextureFromImage(Cog.Image image)
        {
            return new XnaTexture(image);
        }

        public override Texture LoadTexture(string filename)
        {
            return LoadTexture(File.ReadAllBytes(filename));
        }

        protected override void InitializeBlendModes()
        {
            AdditiveBlend = new AdditiveBlend();
            AlphaBlend = new AlphaBlend();
        }

        protected override void InitializeShaders()
        {
            DefaultShader = new XnaShader();
        }

        public override GlslShader LoadGlslShader(string vertexShaderSource, string fragmentShaderSource)
        {
            throw new NotSupportedException("XnaRenderer does not support GLSL shaders!");
        }
    }
}
