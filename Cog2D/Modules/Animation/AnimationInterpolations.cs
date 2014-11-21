using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Animation
{
    public static class AnimationInterpolations
    {
        public static Func<float, float> Linear,
            Sine;

        public static void Initialize()
        {
            Linear = t => t;
            Sine = t => (Mathf.Sin(t * Mathf.Pi - Mathf.HalfPi) + 1f) / 2f;
        }
    }
}
