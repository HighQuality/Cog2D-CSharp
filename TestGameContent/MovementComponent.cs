using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cog;
using Cog.Modules;
using Cog.Modules.Content;
using Cog.Modules.EventHost;

namespace TestGame
{
    public class MovementComponent : Cog.Modules.Content.LinearPhysicsComponent
    {
        public KeyCapture Left,
            Right,
            Up,
            Down;
        public float MovementForce = 300f,
            MaxSpeed = 200f;

        new public static MovementComponent RegisterOn(GameObject obj)
        {
            var c = new MovementComponent(obj);
            obj.RegisterEvent<PhysicsUpdateEvent>(0, c.PhysicsUpdate);
            return c;
        }

        public MovementComponent(GameObject gameObject)
            : base(gameObject)
        {
        }

        public override void PhysicsUpdate(PhysicsUpdateEvent ev)
        {
            if (Left != null && Left.IsDown)
                Speed.X -= MovementForce * ev.DeltaTime;
            if (Right != null && Right.IsDown)
                Speed.X += MovementForce * ev.DeltaTime;
            if (Up != null && Up.IsDown)
                Speed.Y -= MovementForce * ev.DeltaTime;
            if (Down != null && Down.IsDown)
                Speed.Y += MovementForce * ev.DeltaTime;

            Speed *= Mathf.Max(0f, 1f - ev.DeltaTime * 3f);

            float newSpeed = Math.Min(MaxSpeed, Speed.Length);
            Speed = Speed.Unit * newSpeed;

            base.PhysicsUpdate(ev);
        }
    }
}
