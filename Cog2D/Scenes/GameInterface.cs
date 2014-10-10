using Cog2D.Interface;
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
    }
}
