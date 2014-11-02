using Cog.Modules.EventHost;
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

        public override void PhysicsUpdate(PhysicsUpdateEvent ev)
        {
            LocalCoord += new Vector2(Speed.X * ev.DeltaTime, Speed.Y * ev.DeltaTime);

            base.PhysicsUpdate(ev);
        }
    }
}
