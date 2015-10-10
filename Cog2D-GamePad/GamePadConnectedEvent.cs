using Cog;
using Cog.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog2D.GamePad
{
    public class GamePadConnectedEvent : EventParameters
    {
        public GamePadIndex Index { get; private set; }

        public GamePadConnectedEvent(GamePadIndex index)
            : base(null)
        {
            this.Index = index;
        }
    }
}
