using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D3DRenderer
{
    public class D3DRenderer : RenderModule
    {
        public override Window CreateWindow(string title, int width, int height, WindowStyle style)
        {
            return new D3DWindow(title, width, height, style);
        }

        public override RenderTexture CreateRenderTexture(int width, int height)
        {
            throw new NotImplementedException();
        }

        protected override void InitializeBlendModes()
        {
            AlphaBlend = new AlphaBlend();
            AdditiveBlend = new AlphaBlend();
        }

        protected override void InitializeShaders()
        {
            throw new NotImplementedException();
        }

        public override Texture LoadTexture(byte[] data)
        {
            return new D3DTexture(data);
        }

        public override Texture LoadTexture(string filename)
        {
            return LoadTexture(File.ReadAllBytes(filename));
        }

        public override Texture TextureFromImage(Cog.Image image)
        {
            return new D3DTexture(image);
        }

        public override GlslShader LoadGlslShader(string vertexShaderSource, string fragmentShaderSource)
        {
            throw new InvalidOperationException("D3DRenderer does not support GLSL shaders!");
        }
    }
}
