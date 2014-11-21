using Cog.Modules.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Animation
{
    public class Animation
    {
        public List<Keyframe> Keyframes = new List<Keyframe>();

        public void AddKeyframe(float duration, Vector2? position, Func<float, float> positionInterpolation, Vector2? scale, Func<float, float> scaleInterpolation, Angle? rotation, Func<float, float> rotationInterpolation)
        {
            Keyframe keyframe;

            // Inherit properties from previous keyframe
            if (Keyframes.Count > 0)
                keyframe = Keyframes[Keyframes.Count - 1];
            else
            {
                keyframe = new Keyframe();

                // Default values
                keyframe.PositionInterpolationFrom = AnimationInterpolations.Linear;
                keyframe.ScaleInterpolationFrom = AnimationInterpolations.Linear;
                keyframe.RotationInterpolationFrom = AnimationInterpolations.Linear;
                keyframe.Scale = Vector2.One;
            }

            keyframe.Duration = duration;

            if (position != null)
                keyframe.Position = position.Value;
            if (scale != null)
                keyframe.Scale = scale.Value;
            if (rotation != null)
                keyframe.Rotation = rotation.Value;

            if (positionInterpolation != null)
                keyframe.PositionInterpolationFrom = positionInterpolation;
            if (scaleInterpolation != null)
                keyframe.ScaleInterpolationFrom = scaleInterpolation;
            if (rotationInterpolation != null)
                keyframe.RotationInterpolationFrom = rotationInterpolation;

            Keyframes.Add(keyframe);
        }
    }
}
