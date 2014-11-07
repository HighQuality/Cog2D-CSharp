using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Resources
{
    public interface IResource : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether or not this resource was generated during runtime
        /// </summary>
        bool IsDynamic { get; }
        /// <summary>
        /// Gets a value whether or not this resource is currently loaded into memory
        /// </summary>
        bool IsLoaded { get; }
        /// <summary>
        /// Gets the Resource Container this resource belongs to, if any
        /// </summary>
        ResourceContainer Container { get; }
    }
}
