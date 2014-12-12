using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    class SynchronizedListRemoveAt : SynchronizedListMessage
    {
        public int RemoveIndex;

        public SynchronizedListRemoveAt(GameObject baseObject, ushort synchronizationId, int removeIndex)
            : base(baseObject, synchronizationId)
        {
            this.RemoveIndex = removeIndex;
        }

        public override void ExecuteCommand(ISynchronizedList list, ITypeWriter typeSerializer)
        {
            list.RemoveCommand(RemoveIndex);
        }
    }
}
