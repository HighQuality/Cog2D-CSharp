using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Networking
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MessageExecutionAttribute : Attribute
    {
        public bool Immediate;
    }
}
