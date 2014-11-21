using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    internal class InitializationData
    {
        public FieldInfo[] SynchronizedFields;
        public object[] SynchronizedValues;
    }
}
