using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog
{
    public struct Permissions
    {
        public static Permissions FullPermissions { get { Permissions p; p.CreateGlobalObjects = true; return p; } }
        public static Permissions DefaultClientPermissions { get { Permissions p; p.CreateGlobalObjects = false; return p; } }

        /// <summary>
        /// Whether or not this client can instruct the server to create a global object
        /// </summary>
        public bool CreateGlobalObjects;
    }
}
