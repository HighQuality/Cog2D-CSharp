using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.EventHost
{
    public static class Mouse
    {
        public enum Button
        {
            Left,
            Middle,
            Right
        }

        private static bool[] buttonState;
        private static Action[] buttonUpCallbacks;

        public static Vector2 Location { get; set; }

        public static void Initialize()
        {
            buttonState = new bool[Enum.GetValues(typeof(Button)).Length];
            buttonUpCallbacks = new Action[buttonState.Length];
        }

        /// <summary>
        /// Only to be called by renderer module
        /// </summary>
        public static void SetDown(Mouse.Button button)
        {
            if (buttonState[(int)button])
                throw new InvalidOperationException("The button is already down!");
            buttonState[(int)button] = true;
            var ev = new ButtonDownEvent(null, button, Mouse.Location);
            Engine.SceneHost.TriggerButton(ev);
            buttonUpCallbacks[(int)button] = ev.ButtonUpCallback;
        }

        /// <summary>
        /// Only to be called by renderer module
        /// </summary>
        public static void SetReleased(Mouse.Button button)
        {
            if (!buttonState[(int)button])
                throw new InvalidOperationException("The button is already up!");
            buttonState[(int)button] = false;
            if (buttonUpCallbacks[(int)button] != null)
            {
                buttonUpCallbacks[(int)button]();
                buttonUpCallbacks[(int)button] = null;
            }
        }

        public static bool IsKeyDown(Button button)
        {
            return buttonState[(int)button];
        }

        public static bool IsKeyUp(Button button)
        {
            return !buttonState[(int)button];
        }
    }
}
