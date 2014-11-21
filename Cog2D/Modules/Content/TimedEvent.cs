using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public class TimedEvent
    {
        public bool IsCancelled { get; private set; }
        public double Time { get; private set; }
        public Action<float> Action { get; private set; }

        public TimedEvent(Action<float> action, double scheduledFor)
        {
            this.Action = action;
            this.Time = scheduledFor;
        }

        public void Cancel()
        {
            IsCancelled = true;
        }
    }
}
