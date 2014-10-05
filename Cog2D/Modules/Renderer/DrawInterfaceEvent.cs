using Cog.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Renderer
{
    public class DrawInterfaceEvent : EventParameters, ICloneable<DrawInterfaceEvent>
    {
        public IRenderTarget RenderTarget { get; private set; }

        public DrawInterfaceEvent(Object sender, IRenderTarget renderTarget)
            : base(sender)
        {
            this.RenderTarget = renderTarget;
        }

        public DrawInterfaceEvent(DrawInterfaceEvent clone)
            : this(clone.Sender, clone.RenderTarget)
        {
        }

        public DrawInterfaceEvent Clone()
        {
            return new DrawInterfaceEvent(this);
        }
    }
}
