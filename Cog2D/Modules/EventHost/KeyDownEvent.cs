using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.EventHost
{
    public class KeyDownEvent : EventParameters
    {
        public Keyboard.Key Key { get; private set; }
        public Action KeyUpEvent;

        public KeyDownEvent(Object sender, Keyboard.Key key)
            : base(sender)
        {
            this.Key = key;
        }
    }
}
