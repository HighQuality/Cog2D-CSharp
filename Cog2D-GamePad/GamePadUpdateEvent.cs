using Cog;
using Cog.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog2D.GamePad
{
    public class GamePadUpdateEvent : EventParameters, IGamePadState
    {
        public GamePadState State { get; private set; }

        public Vector2 LeftStick { get { return State.LeftStick; } }
        public Vector2 RightStick { get { return State.RightStick; } }
        public float LeftTrigger { get { return State.LeftTrigger; } }
        public float RightTrigger { get { return State.RightTrigger; } }
        public bool IsConnected { get { return State.IsConnected; } }
        public GamePadIndex Index { get { return State.Index; } }

        public bool ButtonIsDown(GamePadButton button)
        {
            return State.ButtonIsDown(button);
        }

        public bool ButtonIsUp(GamePadButton button)
        {
            return State.ButtonIsUp(button);
        }

        public GamePadUpdateEvent(GamePadState state)
            : base(null)
        {
            this.State = state;
        }
    }
}
