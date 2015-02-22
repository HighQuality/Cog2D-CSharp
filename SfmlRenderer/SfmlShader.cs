using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.SfmlRenderer
{
    public class SfmlShader : GlslShader
    {
        internal SFML.Graphics.Shader Shader;

        internal SfmlShader(string vertexShader, string fragmentShader)
        {
            Shader = new SFML.Graphics.Shader(new MemoryStream(Encoding.UTF8.GetBytes(vertexShader)), new MemoryStream(Encoding.UTF8.GetBytes(fragmentShader)));
        }

        public override void SetUniform(string name, int value)
        {
            Shader.SetParameter(name, value);
        }

        public override void SetUniform(string name, float value)
        {
            Shader.SetParameter(name, value);
        }

        public override void SetUniform(string name, Vector2 value)
        {
            Shader.SetParameter(name, value.X, value.Y);
        }

        public override void SetUniform(string name, float x, float y)
        {
            Shader.SetParameter(name, x, y);
        }

        public override void SetUniform(string name, float x, float y, float z)
        {
            Shader.SetParameter(name, x, y, z);
        }

        public override void SetUniform(string name, float x, float y, float z, float w)
        {
            Shader.SetParameter(name, x, y, z, w);
        }

        public override void SetUniform(string name, Color color)
        {
            Shader.SetParameter(name, new SFML.Graphics.Color((byte)color.R, (byte)color.G, (byte)color.B, (byte)color.A));
        }

        public override void SetUniform(string name, Texture texture)
        {
            Shader.SetParameter(name, ((SfmlTexture)texture).Texture);
        }

        protected override void Set()
        {
            var rs = SfmlRenderer.RenderState;
            rs.Shader = Shader;
            SfmlRenderer.RenderState = rs;
        }

        private void Dispose(bool disposed)
        {
            if (disposed)
            {
                Shader.Dispose();
            }
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SfmlShader()
        {
            Dispose(false);
        }
    }
}
