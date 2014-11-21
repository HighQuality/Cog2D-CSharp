using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Animation
{
    public struct Keyframe
    {
        public Vector2 Position,
            Scale;
        public Angle Rotation;
        public double Duration;

        public Func<float, float> PositionInterpolationFrom,
            ScaleInterpolationFrom,
            RotationInterpolationFrom;
    }
}
