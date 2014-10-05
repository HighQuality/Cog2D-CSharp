using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public class LinearPhysicsComponent : ObjectComponent
    {
        public Vector2 Speed;

        public override void PhysicsUpdate(float deltaTime)
        {
            WorldCoord += new Vector2(Speed.X * deltaTime, Speed.Y * deltaTime);

            base.Update(deltaTime);
        }
    }
}
