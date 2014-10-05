using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.EventHost
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ComponentEventAttribute : Attribute
    {
        public bool RequireOverride;

        public ComponentEventAttribute(bool requireOverride)
        {
            this.RequireOverride = requireOverride;
        }
    }
}
