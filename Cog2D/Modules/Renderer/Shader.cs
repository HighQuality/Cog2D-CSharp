using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cog.Modules.Renderer
{
    public abstract class Shader : IDisposable
    {
        public abstract void Dispose();

        /// <summary>
        /// Sets the currently active shader.
        /// Returns an activation context that must be disposed and resets the blend mode to the previous one
        /// </summary>
        public ActivationContext Activate()
        {
            var oldShader = Engine.Renderer.Shader;
            Engine.Renderer.Shader = this;

            Set();

            return new ActivationContext(() =>
            {
                Engine.Renderer.Shader = oldShader;
                Engine.Renderer.Shader.Set();
            });
        }

        internal void ForceActivate()
        {
            Set();
        }

        protected abstract void Set();
    }
}
