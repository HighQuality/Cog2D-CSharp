using Square.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square
{
    public struct Keys
    {
        public bool this[Keyboard.Key index]
        {
            get
            {
                if (Engine.IsServer)
                    return false;
                return Engine.Window.IsKeyDown(index);
            }
        }
    }

    public static class Keyboard
    {
        public enum Key
        {
            Left,
            Right,
            Up,
            Down,
            LShift,
            RShift,
            LCtrl,
            RCtrl,
            LAlt,
            RAlt,
            Space,
            Escape,
            Tab,

            Num0,
            Num1,
            Num2,
            Num3,
            Num4,
            Num5,
            Num6,
            Num7,
            Num8,
            Num9,

            A,
            B,
            C,
            D,
            E,
            F,
            G,
            H,
            I,
            J,
            K,
            L,
            M,
            N,
            O,
            P,
            Q,
            R,
            S,
            T,
            U,
            V,
            W,
            X,
            Y,
            Z,

            F1,
            F2,
            F3,
            F4,
            F5,
            F6,
            F7,
            F8,
            F9,
            F10,
            F11,
            F12,

            Unknown,
            Count
        }
    }
}
