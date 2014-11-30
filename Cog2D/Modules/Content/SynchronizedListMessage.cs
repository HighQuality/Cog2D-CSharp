using Cog.Modules.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    abstract class SynchronizedListMessage : NetworkMessage
    {
        public GameObject Object;
        public ushort SynchronizationId;

        public SynchronizedListMessage(GameObject baseObject, ushort synchronizationId)
        {
            this.Object = baseObject;
            this.SynchronizationId = synchronizationId;
        }

        public override void Received()
        {
            var field = GameObject.GetSynchronizedField(Object.GetType(), SynchronizationId);
            var list = (ISynchronizedList)field.GetValue(Object);
            var typeSerializer = TypeSerializer.GetTypeWriter(field.FieldType.GenericTypeArguments[0]);
            ExecuteCommand(list, typeSerializer);
        }

        public abstract void ExecuteCommand(ISynchronizedList list, ITypeWriter typeSerializer);
    }
}
