using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cog.Modules.Networking
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ClientOnlyAttribute : Attribute
    {
    }
}
