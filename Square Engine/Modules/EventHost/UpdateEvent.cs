using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.EventHost
{
    public class UpdateEvent : EventParameters
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
    }
}
