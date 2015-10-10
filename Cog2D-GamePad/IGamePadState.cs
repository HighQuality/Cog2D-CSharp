using Cog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cog2D.GamePad
{
    public interface IGamePadState
    {
        GamePadIndex Index { get; }
        Vector2 LeftStick { get; }
        Vector2 RightStick { get; }
        float LeftTrigger { get; }
        float RightTrigger { get; }
        bool IsConnected { get; }

        bool ButtonIsDown(GamePadButton button);
        bool ButtonIsUp(GamePadButton button);
    }
}
