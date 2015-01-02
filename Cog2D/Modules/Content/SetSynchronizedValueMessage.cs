using Cog.Modules.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    class SetSynchronizedValueMessage : NetworkMessage
    {
        public GameObject Object;
        public ushort SynchronizationId;
        public byte[] Data;

        public SetSynchronizedValueMessage()
        {
        }

        public override void Received()
        {
            if (Object != null)
            {
                var field = GameObject.GetSynchronizedField(Object.GetType(), SynchronizationId);
                var converter = TypeSerializer.GetTypeWriter(field.FieldType.GetGenericArguments()[0]);

                ISynchronized prop = (ISynchronized)field.GetValue(Object);
                using (MemoryStream stream = new MemoryStream(Data))
                    using (BinaryReader reader = new BinaryReader(stream))
                        prop.Deserialize(reader);
                field.SetValue(Object, prop);
            }
            else
            {
                Debug.Error("Tried to set synchronized value of object that couldn't be found!");
            }
        }
    }
}
