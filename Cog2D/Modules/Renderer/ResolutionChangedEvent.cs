using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cog.Modules.EventHost;

namespace Cog.Modules.Renderer
{
    public class ResolutionChangedEvent : EventParameters
    {
        public readonly float Width,
            Height;

        public ResolutionChangedEvent(float width, float height)
            : base(null)
        {
            this.Width = width;
            this.Height = height;
        }
    }
}
