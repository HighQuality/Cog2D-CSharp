using Cog.Interface;
using Cog.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Scenes
{
    public class GameInterface : InterfaceElement
    {
        internal GameInterface()
            : base(null, new Vector2(0f, 0f))
        {
            MinimumSize = Engine.Resolution;
        }

        internal bool Press(Vector2 position, ButtonDownEvent ev)
        {
            if (position.X < 0f || position.Y < 0f || position.X >= Size.X || position.Y >= Size.Y)
                return false;

            foreach (var child in EnumerateChildren())
                if (child.PredicatePress(ev.Button, position - child.Location))
                    if (child.TriggerPress(position - child.Location, ev))
                        return true;

            return false;
        }
    }
}
