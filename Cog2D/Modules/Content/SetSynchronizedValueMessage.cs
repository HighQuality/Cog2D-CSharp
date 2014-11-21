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
                var converter = TypeSerializer.GetTypeWriter(field.FieldType.GenericTypeArguments[0]);

                using (MemoryStream stream = new MemoryStream(Data))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        ISynchronized prop = (ISynchronized)field.GetValue(Object);
                        object receivedValue = converter.GenericRead(reader);
                        prop.ForceSet(receivedValue);
                        field.SetValue(Object, prop);
                    }
                }
            }
            else
            {
                Console.WriteLine("Object does not exist");
                //throw new Exception("Tried to set synchronized value on an object that does not exist!");
            }
        }
    }
}
