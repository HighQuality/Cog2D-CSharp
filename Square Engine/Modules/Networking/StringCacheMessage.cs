using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.Networking
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
            ushort id = (ushort)Sender.CachedStrings.Count;
            Sender.CachedStrings.Add(CachedString);
            Sender.CachedStringDictionary.Add(CachedString, id);
        }
    }
}
