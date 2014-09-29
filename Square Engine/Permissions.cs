using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square
{
    public struct Permissions
    {
        /// <summary>
        /// Whether or not this client can instruct the server to create a global object
        /// </summary>
        public bool CreateGlobalObjects { get; private set; }
        /// <summary>
        /// Whether or not this client can send a message directly to another client
        /// </summary>
        public bool SendRelay { get; private set; }
    }
}
