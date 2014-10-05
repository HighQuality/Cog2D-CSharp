using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.Networking
{
    public enum StringSendType
    {
        DynamicUShort,
        Cached
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class StringPropertiesAttribute : Attribute
    {
        public StringSendType Type;

        public StringPropertiesAttribute(StringSendType sendType)
        {
            this.Type = sendType;
        }
    }
}
