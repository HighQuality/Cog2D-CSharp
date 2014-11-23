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
        void Initialize(GameObject obj, ushort synchronizationId, object value);
        GameObject BaseObject { get; set; }
        ushort SynchronizationId { get; set; }
        void ForceSet(object value);
        object GenericGet();

        void Serialize(BinaryWriter writer);
        void Deserialize(BinaryReader reader);
    }

    public struct Synchronized<T> : ISynchronized
    {
        public GameObject BaseObject { get; set; }
        public ushort SynchronizationId { get; set; }

        private T _value;

        public Synchronized(T value)
            : this()
        {
            this._value = value;
        }

        public void Initialize(GameObject obj, ushort synchronizationId, object value)
        {
            if (value != null)
                _value = (T)value;
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
                        TypeSerializer.GetTypeWriter(GetType().GenericTypeArguments[0]).GenericWrite(_value, writer);
                        message.Data = stream.ToArray();
                    }
                }
                
                BaseObject.Send(message);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            TypeSerializer.GetTypeWriter(GetType().GenericTypeArguments[0]).GenericWrite(_value, writer);
        }

        public void Deserialize(BinaryReader reader)
        {
            _value = (T)TypeSerializer.GetTypeWriter(GetType().GenericTypeArguments[0]).GenericRead(reader);
        }

        public void ForceSet(object value)
        {
            this._value = (T)value;
        }

        public object GenericGet()
        {
            return _value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
