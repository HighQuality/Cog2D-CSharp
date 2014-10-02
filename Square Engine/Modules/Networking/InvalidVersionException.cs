using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.Networking
{
    public class InvalidVersionException : Exception
    {
        public InvalidVersionException(string message)
            : base(message)
        {
        }
    }
}
