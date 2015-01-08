using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D3DRenderer
{
    public abstract class DXResource : IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DXResource()
        {
            Dispose(false);
        }

        public abstract void Dispose(bool disposing);
    }
}
