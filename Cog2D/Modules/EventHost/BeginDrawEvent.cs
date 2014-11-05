using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.EventHost
{
    public class BeginDrawEvent : EventParameters
    {
        public IRenderTarget RenderTarget { get; private set; }

        public BeginDrawEvent(Object sender, IRenderTarget renderTarget)
            : base(sender)
        {
            this.RenderTarget = renderTarget;
        }
    }
}
