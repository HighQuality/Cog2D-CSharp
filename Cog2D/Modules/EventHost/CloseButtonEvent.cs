using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.EventHost
{
    public class CloseButtonEvent : EventParameters
    {
        public CloseButtonEvent(Object sender)
            : base(sender)
        {
        }
    }
}
