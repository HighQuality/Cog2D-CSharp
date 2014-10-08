using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public class ComponentSerializer
    {
        Action<ObjectComponent, BinaryWriter>[] writers;
        Action<ObjectComponent, BinaryReader>[] readers;
        public Type Type { get; private set; }
        public UInt16 Id { get; private set; }

        public ComponentSerializer(Type type, Action<ObjectComponent, BinaryWriter>[] writers, Action<ObjectComponent, BinaryReader>[] readers, UInt16 id)
        {
            this.Type = type;
            this.writers = writers;
            this.readers = readers;
            this.Id = id;
        }

        public void Serialize(ObjectComponent component, BinaryWriter writer)
        {
            for (int i = 0; i < this.writers.Length; i++)
                this.writers[i](component, writer);
        }

        public void Deserialize(ObjectComponent component, BinaryReader reader)
        {
            for (int i = 0; i < this.readers.Length; i++)
                this.readers[i](component, reader);
        }
    }
}
