using Cog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaInput = Microsoft.Xna.Framework.Input;

namespace Cog2D.GamePad
{
    public struct GamePadState : IGamePadState
    {
        private Vector2 leftStick,
            rightStick;
        private float leftTrigger,
            rightTrigger;
        private bool[] buttons;
        private bool isConnected;
        private GamePadIndex index;

        public GamePadIndex Index { get { return index; } }
        public Vector2 LeftStick { get { return leftStick; } }
        public Vector2 RightStick { get { return rightStick; } }
        public float LeftTrigger { get { return leftTrigger; } }
        public float RightTrigger { get { return rightTrigger; } }
        public bool IsConnected { get { return isConnected; } }

        public bool ButtonIsDown(GamePadButton button)
        {
            if (buttons == null)
                return false;
            return buttons[(int)button];
        }

        public bool ButtonIsUp(GamePadButton button)
        {
            if (buttons == null)
                return false;
            return !buttons[(int)button];
        }

        internal static GamePadState New(GamePadIndex index)
        {
            XnaInput.GamePadState state;
            switch (index)
            {
                case GamePadIndex.One:
                    state = XnaInput.GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One, XnaInput.GamePadDeadZone.Circular);
                    break;
                case GamePadIndex.Two:
                    state = XnaInput.GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.Two, XnaInput.GamePadDeadZone.Circular);
                    break;
                case GamePadIndex.Three:
                    state = XnaInput.GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.Three, XnaInput.GamePadDeadZone.Circular);
                    break;
                case GamePadIndex.Four:
                    state = XnaInput.GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.Four, XnaInput.GamePadDeadZone.Circular);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("index");
            }

            Vector2 leftStick = new Vector2(state.ThumbSticks.Left.X, -state.ThumbSticks.Left.Y);
            Vector2 rightStack = new Vector2(state.ThumbSticks.Right.X, -state.ThumbSticks.Right.Y);

            var newState = new GamePadState
            {
                buttons = new bool[(int)GamePadButton.Count],
                leftStick = leftStick,
                rightStick = rightStack,
                leftTrigger = state.Triggers.Left,
                rightTrigger = state.Triggers.Right,
                isConnected = state.IsConnected,
                index = index
            };

            newState.buttons[(int)GamePadButton.LeftShoulderButton] = state.Buttons.LeftShoulder == XnaInput.ButtonState.Pressed;
            newState.buttons[(int)GamePadButton.RightShoulderButton] = state.Buttons.RightShoulder == XnaInput.ButtonState.Pressed;
            newState.buttons[(int)GamePadButton.LeftStick] = state.Buttons.LeftStick == XnaInput.ButtonState.Pressed;
            newState.buttons[(int)GamePadButton.RightStick] = state.Buttons.RightStick == XnaInput.ButtonState.Pressed;
            newState.buttons[(int)GamePadButton.Back] = state.Buttons.Back == XnaInput.ButtonState.Pressed;
            newState.buttons[(int)GamePadButton.Start] = state.Buttons.Start == XnaInput.ButtonState.Pressed;
            newState.buttons[(int)GamePadButton.A] = state.Buttons.A == XnaInput.ButtonState.Pressed;
            newState.buttons[(int)GamePadButton.B] = state.Buttons.B == XnaInput.ButtonState.Pressed;
            newState.buttons[(int)GamePadButton.X] = state.Buttons.X == XnaInput.ButtonState.Pressed;
            newState.buttons[(int)GamePadButton.Y] = state.Buttons.Y == XnaInput.ButtonState.Pressed;
            newState.buttons[(int)GamePadButton.BigButton] = state.Buttons.BigButton == XnaInput.ButtonState.Pressed;
            newState.buttons[(int)GamePadButton.DPadLeft] = state.DPad.Left == XnaInput.ButtonState.Pressed;
            newState.buttons[(int)GamePadButton.DPadRight] = state.DPad.Right == XnaInput.ButtonState.Pressed;
            newState.buttons[(int)GamePadButton.DPadUp] = state.DPad.Up == XnaInput.ButtonState.Pressed;
            newState.buttons[(int)GamePadButton.DPadDown] = state.DPad.Down == XnaInput.ButtonState.Pressed;

            return newState;
        }
    }
}
