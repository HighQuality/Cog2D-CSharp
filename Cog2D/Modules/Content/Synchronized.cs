using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public interface ISynchronized
    {
        void Initialize(GameObject obj, ushort synchronizationId);
        GameObject BaseObject { get; }
        ushort SynchronizationId { get; }

        void Serialize(BinaryWriter writer);
        void Deserialize(BinaryReader reader);
    }

    public class Synchronized<T> : ISynchronized
    {
        public GameObject BaseObject { get; set; }
        public ushort SynchronizationId { get; set; }

        private T _value;
        private ITypeWriter serializer;

        private Synchronized()
        {
            serializer = TypeSerializer.GetTypeWriter(GetType().GenericTypeArguments[0]);
        }

        public void Initialize(GameObject obj, ushort synchronizationId)
        {
            BaseObject = obj;
            SynchronizationId = synchronizationId;
        }

        public T Value
        {
            get { return _value; }

            set
            {
                _value = value;

                SetSynchronizedValueMessage message = new SetSynchronizedValueMessage();
                message.Object = BaseObject;
                message.SynchronizationId = SynchronizationId;
                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        serializer.GenericWrite(_value, writer);
                        message.Data = stream.ToArray();
                    }
                }
                
                BaseObject.Send(message);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            serializer.GenericWrite(_value, writer);
        }

        public void Deserialize(BinaryReader reader)
        {
            _value = (T)serializer.GenericRead(reader);
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
