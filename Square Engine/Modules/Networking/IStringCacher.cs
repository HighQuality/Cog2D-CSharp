using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.Networking
{
    interface IStringCacher
    {
        ushort GetIdFromString(string value);
        string GetStringFromId(ushort id);
    }
}
