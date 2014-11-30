using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    class SynchronizedListInsert : SynchronizedListMessage
    {
        public int InsertIndex;
        public byte[] Data;

        public SynchronizedListInsert(GameObject baseObject, ushort synchronizationId, int insertIndex, byte[] data)
            : base(baseObject, synchronizationId)
        {
            this.InsertIndex = insertIndex;
            this.Data = data;
        }

        public override void ExecuteCommand(ISynchronizedList list, ITypeWriter typeSerializer)
        {
            list.InsertCommand(typeSerializer.FromBytes(Data), InsertIndex);
        }
    }
}
