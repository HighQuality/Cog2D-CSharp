using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    class SynchronizedListAdd : SynchronizedListMessage
    {
        public byte[] Data;

        public SynchronizedListAdd(GameObject baseObject, ushort synchronizationId, byte[] data)
            : base(baseObject, synchronizationId)
        {
            this.Data = data;
        }

        public override void ExecuteCommand(ISynchronizedList list, ITypeWriter typeSerializer)
        {
            list.AddCommand(typeSerializer.FromBytes(Data));
        }
    }
}
