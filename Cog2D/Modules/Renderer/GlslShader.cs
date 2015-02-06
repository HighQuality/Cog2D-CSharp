using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cog.Modules.Renderer
{
    public abstract class GlslShader : Shader
    {
        public abstract void SetUniform(string name, int value);
        public abstract void SetUniform(string name, float value);
        public abstract void SetUniform(string name, Vector2 value);
        public abstract void SetUniform(string name, Color value);
        public abstract void SetUniform(string name, Texture value);
        public abstract void SetUniform(string name, float x, float y);
        public abstract void SetUniform(string name, float x, float y, float z);
        public abstract void SetUniform(string name, float x, float y, float z, float w);
    }
}
