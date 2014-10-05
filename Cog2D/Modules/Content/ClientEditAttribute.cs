using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ClientEditAttribute : Attribute
    {
        public bool CanEdit;
        public bool RequireOwner;
    }
}
