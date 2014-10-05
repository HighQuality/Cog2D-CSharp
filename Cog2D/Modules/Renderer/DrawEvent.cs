using Cog.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Renderer
{
    public class DrawEvent : EventParameters, ICloneable<DrawEvent>
    {
        public IRenderTarget RenderTarget { get; private set; }

        public DrawEvent(Object sender, IRenderTarget renderTarget)
            : base(sender)
        {
            this.RenderTarget = renderTarget;
        }

        public DrawEvent(DrawEvent clone)
            : this(clone.Sender, clone.RenderTarget)
        {
        }

        public DrawEvent Clone()
        {
            return new DrawEvent(this);
        }
    }
}
