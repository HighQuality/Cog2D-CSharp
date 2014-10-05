using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.EventHost
{
    public interface IEventListener
    {
        IEvent IEvent { get; }
        bool IsCancelled { get; }
        void Cancel();
    }
}
