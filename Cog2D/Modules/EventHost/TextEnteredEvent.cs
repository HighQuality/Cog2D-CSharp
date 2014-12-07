using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.EventHost
{
    public class TextEnteredEvent : EventParameters
    {
        public char Character;

        public TextEnteredEvent(char character)
            : base(null)
        {
            this.Character = character;
        }
    }
}
