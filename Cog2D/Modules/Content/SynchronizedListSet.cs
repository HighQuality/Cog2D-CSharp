using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    class SynchronizedListSet : SynchronizedListMessage
    {
        public ushort Index;
        public byte[] Data;

        public SynchronizedListSet(GameObject baseObject, ushort synchronizationId, ushort index, byte[] data)
            : base(baseObject, synchronizationId)
        {
            this.Index = index;
            this.Data = data;
        }

        public override void ExecuteCommand(ISynchronizedList list, ITypeWriter typeSerializer)
        {
            list.SetCommand(Index, typeSerializer.FromBytes(Data));
        }
    }
}
