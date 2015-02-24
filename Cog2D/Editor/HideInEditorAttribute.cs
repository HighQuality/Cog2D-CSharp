using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cog.Editor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class HideInEditorAttribute : Attribute
    {
    }
}
