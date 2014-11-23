using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    /// <summary>
    /// A Synchronized value that is resolved on read-time through an identifier.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SynchronizedDR<T> : ISynchronized
        where T : IIdentifier
    {
        public GameObject BaseObject { get; set; }
        public ushort SynchronizationId { get; set; }

        public long Identifier { get; private set; }

        public T Value
        {
            get
            {
                return Engine.Resolve<T>(Identifier);
            }
            set
            {
                if (value != null)
                    Identifier = value.Id;
                else
                    Identifier = 0;

                SetSynchronizedValueMessage msg = new SetSynchronizedValueMessage();
                msg.Object = BaseObject;
                msg.SynchronizationId = SynchronizationId;
                msg.Data = BitConverter.GetBytes(Identifier);
                BaseObject.Send(msg);
            }
        }

        private SynchronizedDR()
        {
        }

        public void Initialize(GameObject obj, ushort synchronizationId)
        {
            BaseObject = obj;
            SynchronizationId = synchronizationId;
        }

        public object GenericGet()
        {
            return (object)Identifier;
        }

        public void ForceSet(object value)
        {
            Identifier = (long)value;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Identifier);
        }

        public void Deserialize(BinaryReader reader)
        {
            Identifier = BitConverter.ToInt64(reader.ReadBytes(sizeof(long)), 0);
        }
    }
}
