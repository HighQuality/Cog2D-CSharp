using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public interface ISynchronized { GameObject BaseObject { get; set; } ushort SynchronizationId { get; set; } ITypeWriter TypeWriter { get; set; } void ForceSet(object value); object GenericGet(); }

    public struct Synchronized<T> : ISynchronized
    {
        public GameObject BaseObject { get; set; }
        public ushort SynchronizationId { get; set; }
        public ITypeWriter TypeWriter { get; set; }

        private T _value;

        public Synchronized(T value)
            : this()
        {
            this._value = value;
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
                        TypeWriter.GenericWrite(_value, writer);
                        message.Data = stream.ToArray();
                    }
                }
                
                BaseObject.Send(message);
            }
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
