using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square
{
    public class NoSerializerException : Exception
    {
        public NoSerializerException(string message)
            : base(message)
        {
        }
    }
}
