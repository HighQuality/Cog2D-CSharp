using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cog.Modules.Renderer;
using Cog.Modules.EventHost;

namespace Cog.Modules.Content
{
    public class FrameAnimationComponent
    {
        private SpriteComponent sprite;

        public float FramesPerSecond;
        public float Frame;
        public Texture[] Frames;

        public static FrameAnimationComponent RegisterOn(SpriteComponent c, float framesPerSecond, params Texture[] frames)
        {
            return new FrameAnimationComponent(c, framesPerSecond, frames);
        }

        private FrameAnimationComponent(SpriteComponent c, float framesPerSecond, Texture[] frames)
        {
            sprite = c;
            FramesPerSecond = framesPerSecond;
            Frames = frames;

            c.GameObject.RegisterEvent<UpdateEvent>(0, Update);
        }

        private void Update(UpdateEvent ev)
        {
            Frame += FramesPerSecond * ev.DeltaTime;
            sprite.Texture = Frames[(int)Frame % Frames.Length];
        }
    }
}
