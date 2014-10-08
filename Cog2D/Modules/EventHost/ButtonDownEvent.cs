using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.EventHost
{
    public class ButtonDownEvent : EventParameters
    {
        public Mouse.Button Button;
        public Vector2 Position;
        public Action ButtonUpCallback;

        public ButtonDownEvent(Object sender, Mouse.Button button, Vector2 position)
            : base(sender)
        {
            this.Button = button;
            this.Position = position;
        }
    }
}
