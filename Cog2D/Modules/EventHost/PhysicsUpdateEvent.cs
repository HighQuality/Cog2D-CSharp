using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.EventHost
{
    public class PhysicsUpdateEvent : EventParameters, ICloneable<PhysicsUpdateEvent>
    {
        /// <summary>
        /// The time that has elapsed since the last call
        /// </summary>
        public float DeltaTime { get; private set; }

        public PhysicsUpdateEvent(Object sender, float deltaTime)
            : base(sender)
        {
            this.DeltaTime = deltaTime;
        }

        public PhysicsUpdateEvent(PhysicsUpdateEvent clone)
            : this(clone.Sender, clone.DeltaTime)
        {
        }

        public PhysicsUpdateEvent Clone()
        {
            return new PhysicsUpdateEvent(this);
        }
    }
}
