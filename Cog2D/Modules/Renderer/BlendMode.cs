using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Renderer
{
    public abstract class BlendMode
    {
        /// <summary>
        /// Sets the blend mode of the passed window.
        /// Returns an activation context that must be disposed and resets the blend mode to the previous one
        /// </summary>
        public ActivationContext Activate()
        {
            var oldBlendMode = Engine.Renderer.BlendMode;
            Engine.Renderer.BlendMode = this;

            Set();

            return new ActivationContext(() =>
            {
                Engine.Renderer.BlendMode = oldBlendMode;
                Engine.Renderer.BlendMode.Set();
            });
        }

        internal void ForceActivate()
        {
            Set();
        }

        protected abstract void Set();
    }
}
