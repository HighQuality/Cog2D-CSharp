using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Resources
{
    public abstract class Resource : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether or not this resource was generated during runtime
        /// </summary>
        public bool IsDynamic { get; protected set; }
        /// <summary>
        /// Gets a value whether or not this resource is currently loaded into memory
        /// </summary>
        public bool IsLoaded { get; protected set; }
        /// <summary>
        /// Gets the Resource Container this resource belongs to, if any
        /// </summary>
        public ResourceContainer Container { get; internal set; }
        public string File { get; private set; }
        public string Identifier { get { return File + "@" + Container.Name; } }

        /// <summary>
        /// Called when a resource is manually disposed by other means than the finalizer
        /// </summary>
        public abstract void Dispose();

        ~Resource()
        {
            Debug.Info("Disposing Resource {0}!", Identifier);
        }
    }
}
