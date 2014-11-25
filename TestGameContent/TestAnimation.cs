using Cog;
using Cog.Modules.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    class TestAnimation : Animation
    {
        public TestAnimation()
        {
            AddKeyframe(1f, new Vector2(-32, 0f), AnimationInterpolations.Linear, null, null, null, null);
            AddKeyframe(1f, new Vector2(32, 0f), AnimationInterpolations.Linear, null, null, null, null);
        }
    }
}
