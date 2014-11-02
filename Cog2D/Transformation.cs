using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog
{
    public struct DrawTransformation
    {
        public Vector2 WorldCoord,
            ParentWorldCoord;
        public Vector2 WorldScale,
            ParentWorldScale;
        public Angle WorldRotation,
            ParentWorldRotation;
    }
}
