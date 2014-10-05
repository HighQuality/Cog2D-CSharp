using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public interface ISynchronized { }

    public struct Synchronized<T> : ISynchronized
    {
        private T _value;

        public T Value
        {
            get { return _value; }

            set
            {
                _value = value;
            }
        }
    }
}
