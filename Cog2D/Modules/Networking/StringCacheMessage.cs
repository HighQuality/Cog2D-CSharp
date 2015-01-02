using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Networking
{
    [MessageExecution(Immediate = true)]
    class StringCacheMessage : NetworkMessage
    {
        private string CachedString;

        public StringCacheMessage(string cache)
        {
            this.CachedString = cache;
        }

        public override void Received()
        {
            ushort id = (ushort)Client.CachedStrings.Count;
            Client.CachedStrings.Add(CachedString);
            Client.CachedStringDictionary.Add(CachedString, id);
        }
    }
}
