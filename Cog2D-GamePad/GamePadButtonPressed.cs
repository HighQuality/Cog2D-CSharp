using Cog;
using Cog.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog2D.GamePad
{
    public class GamePadButtonPressed : EventParameters
    {
        public GamePadIndex Index { get; private set; }
        public GamePadButton Button { get; private set; }

        public GamePadButtonPressed(GamePadIndex index, GamePadButton button)
            : base(null)
        {
            this.Index = index;
            this.Button = button;
        }
    }
}
