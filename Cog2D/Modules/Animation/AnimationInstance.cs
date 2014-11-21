using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Animation
{
    public class AnimationInstance
    {
        public Animation Animation;
        private double startTime;
        public double Time { get { return Engine.TimeStamp - startTime; } }

        private int currentKeyframe;
        private double previousTime;
        public Keyframe CurrentKeyframe { get { return Animation.Keyframes[currentKeyframe % Animation.Keyframes.Count]; } }
        public Keyframe NextKeyframe { get { return Animation.Keyframes[(currentKeyframe + 1) % Animation.Keyframes.Count]; } }

        public AnimationInstance(Animation animation)
        {
            this.Animation = animation;
            startTime = Engine.TimeStamp;
        }

        public void ApplyTransformation(IAnimationComponent component)
        {
            var time = Time;

            while (time >= previousTime + CurrentKeyframe.Duration)
            {
                previousTime += CurrentKeyframe.Duration;
                currentKeyframe++;
            }

            var currentFrameTime = (previousTime + CurrentKeyframe.Duration) - time;
            var currentProgress = currentFrameTime / CurrentKeyframe.Duration;

            var deltaPosition = (NextKeyframe.Position - CurrentKeyframe.Position);
            var deltaScale = (NextKeyframe.Scale - CurrentKeyframe.Scale);
            var deltaAngle = ((((NextKeyframe.Rotation.Degree - CurrentKeyframe.Rotation.Degree) % 360f) + 540f) % 360f) - 180f;

            component.Object.LocalCoord = CurrentKeyframe.Position + deltaPosition * (float)CurrentKeyframe.PositionInterpolationFrom((float)currentProgress);
            component.Object.LocalScale = CurrentKeyframe.Scale + deltaScale * (float)CurrentKeyframe.ScaleInterpolationFrom((float)currentProgress);
            component.Object.LocalRotation = Angle.FromDegree(CurrentKeyframe.Rotation.Degree + deltaAngle * (float)CurrentKeyframe.RotationInterpolationFrom((float)currentProgress));
        }
    }
}
