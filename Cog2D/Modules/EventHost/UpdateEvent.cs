using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.EventHost
{
    public class UpdateEvent : EventParameters, ICloneable<UpdateEvent>
    {
        /// <summary>
        /// The time that has elapsed since the last frame
        /// </summary>
        public float DeltaTime { get; private set; }

        public UpdateEvent(Object sender, float deltaTime)
            : base(sender)
        {
            this.DeltaTime = deltaTime;
        }

        public UpdateEvent(UpdateEvent clone)
            : this(clone.Sender, clone.DeltaTime)
        {
        }

        public UpdateEvent Clone()
        {
            return new UpdateEvent(this);
        }
    }
}
