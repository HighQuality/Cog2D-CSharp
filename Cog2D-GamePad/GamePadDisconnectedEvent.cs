
using Cog;
using Cog.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog2D.GamePad
{
    public class GamePadDisconnectedEvent : EventParameters
    {
        public GamePadIndex Index { get; private set; }

        public GamePadDisconnectedEvent(GamePadIndex index)
            : base(null)
        {
            this.Index = index;
        }
    }
}
