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
        public ActivationContext ActivateOn(IRenderTarget target)
        {
            var oldBlendMode = target.BlendMode;
            target.BlendMode = this;

            Set(target);

            return new ActivationContext(() =>
            {
                target.BlendMode = oldBlendMode;
                target.BlendMode.Set(target);
            });
        }

        internal void ForceSet(IRenderTarget target)
        {
            target.BlendMode = this;
            Set(target);
        }

        protected abstract void Set(IRenderTarget window);
    }
}
