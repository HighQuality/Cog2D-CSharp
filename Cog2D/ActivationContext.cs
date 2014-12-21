using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog
{
    public sealed class ActivationContext : IDisposable
    {
        private Action disposeAction;

        public ActivationContext(Action disposeAction)
        {
            if (disposeAction == null)
                throw new ArgumentNullException("disposeAction");
            this.disposeAction = disposeAction;
        }

        public void Dispose()
        {
            disposeAction();
            GC.SuppressFinalize(this);
        }

        ~ActivationContext()
        {
            Debug.Error("Activation Context was not Disposed!");
        }
    }
}
