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

        public override bool Equals(object obj)
        {
            var other = (Keyframe)obj;
            return Position == other.Position &&
                Scale == other.Scale &&
                Duration == other.Duration;
        }

        public static bool operator ==(Keyframe first, Keyframe second)
        {
            return first.Equals(second);
        }
        public static bool operator !=(Keyframe first, Keyframe second)
        {
            return !first.Equals(second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
