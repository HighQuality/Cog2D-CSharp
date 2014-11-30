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
        public bool DoLoop { get; set; }
        public Action OnAnimationFinished;
        public bool IsFinished { get; private set; }

        public AnimationInstance(Animation animation)
        {
            if (animation.Keyframes.Count == 0)
                throw new Exception("Animations must have at least one keyframe!");

            this.Animation = animation;
            startTime = Engine.TimeStamp;
            DoLoop = true;
        }

        public void ApplyTransformation(IAnimationComponent component)
        {
            if (IsFinished)
            {
                ApplyFinalTransformation(component);
                return;
            }

            var time = Time;

            while (time >= previousTime + CurrentKeyframe.Duration)
            {
                if (NextKeyframe == Animation.Keyframes[Animation.Keyframes.Count - 1])
                {
                    if (!DoLoop)
                    {
                        IsFinished = true;
                        ApplyFinalTransformation(component);

                        if (OnAnimationFinished != null)
                            OnAnimationFinished();

                        return;
                    }
                }
                previousTime += CurrentKeyframe.Duration;
                currentKeyframe++;
            }

            var currentFrameTime = time - previousTime;
            var currentProgress = currentFrameTime / CurrentKeyframe.Duration;

            var deltaPosition = (NextKeyframe.Position - CurrentKeyframe.Position);
            var deltaScale = (NextKeyframe.Scale - CurrentKeyframe.Scale);
            var deltaAngle = ((((NextKeyframe.Rotation.Degree - CurrentKeyframe.Rotation.Degree) % 360f) + 540f) % 360f) - 180f;

            component.Object.LocalCoord = CurrentKeyframe.Position + deltaPosition * (float)CurrentKeyframe.PositionInterpolationFrom((float)currentProgress);
            component.Object.LocalScale = CurrentKeyframe.Scale + deltaScale * (float)CurrentKeyframe.ScaleInterpolationFrom((float)currentProgress);
            component.Object.LocalRotation = Angle.FromDegree(CurrentKeyframe.Rotation.Degree + deltaAngle * (float)CurrentKeyframe.RotationInterpolationFrom((float)currentProgress));
        }

        private void ApplyFinalTransformation(IAnimationComponent component)
        {
            var lastKeyFrame = Animation.Keyframes[Animation.Keyframes.Count - 1];
            component.Object.LocalCoord = lastKeyFrame.Position;
            component.Object.LocalScale = lastKeyFrame.Scale;
            component.Object.LocalRotation = lastKeyFrame.Rotation;
        }
    }
}
