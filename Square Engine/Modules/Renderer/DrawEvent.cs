using Square.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.Renderer
{
    public class DrawEvent : EventParameters
    {
        public IRenderTarget RenderTarget { get; private set; }

        public DrawEvent(Object sender, IRenderTarget renderTarget)
            : base(sender)
        {
            this.RenderTarget = renderTarget;
        }
    }
}
