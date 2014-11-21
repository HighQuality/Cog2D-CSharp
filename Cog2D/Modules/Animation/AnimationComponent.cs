﻿using Cog.Modules.Content;
using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Animation
{
    public interface IAnimationComponent { AnimationInstance Animation { get; set; } GameObject Object { get; } }
    public interface IAnimated { IAnimationComponent AnimationComponent { get; set; } }

    public class AnimationComponent<T> : IAnimationComponent
        where T : GameObject, IAnimated
    {
        public GameObject Object { get; private set; }
        public AnimationInstance Animation { get; set; }

        private AnimationComponent(T obj)
        {
            this.Object = obj;
            obj.AnimationComponent = this;

            if (obj.OnDraw == null)
                obj.OnDraw = new List<Action<DrawEvent, DrawTransformation>>();

            // Animations need to be applied before any other draw events, insert it into the first index
            obj.OnDraw.Insert(0, Draw);
        }

        private void Draw(DrawEvent ev, DrawTransformation transform)
        {
            if (Animation != null)
                Animation.ApplyTransformation(this);
        }

        public static AnimationComponent<T> RegisterOn(T obj)
        {
            return new AnimationComponent<T>(obj);
        }
    }
}
