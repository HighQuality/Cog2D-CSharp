using Cog.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public class LinearPhysicsComponent
    {
        public GameObject GameObject;
        public Vector2 Speed;

        public static LinearPhysicsComponent RegisterOn(GameObject obj)
        {
            var c = new LinearPhysicsComponent(obj);
            obj.RegisterEvent<PhysicsUpdateEvent>(0, c.PhysicsUpdate);
            return c;
        }

        public LinearPhysicsComponent(GameObject gameObject)
        {
            this.GameObject = gameObject;
        }

        public virtual void PhysicsUpdate(PhysicsUpdateEvent ev)
        {
            GameObject.LocalCoord += new Vector2(Speed.X * ev.DeltaTime, Speed.Y * ev.DeltaTime);
        }
    }
}
