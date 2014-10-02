using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.Networking
{
    public class NetworkHashMismatchException : Exception
    {
        public NetworkHashMismatchException(string message)
            : base(message)
        {
        }
    }
}
