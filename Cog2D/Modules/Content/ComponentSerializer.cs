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
        Action<ObjectComponent, BinaryWriter> writer;
        Action<ObjectComponent, BinaryReader> reader;

        public ComponentSerializer(Action<ObjectComponent, BinaryWriter> writer, Action<ObjectComponent, BinaryReader> reader)
        {
            this.writer = writer;
            this.reader = reader;
        }

        public void Serialize(ObjectComponent component, BinaryWriter writer)
        {
            this.writer(component, writer);
        }

        public void Deserialize(ObjectComponent component, BinaryReader reader)
        {
            this.reader(component, reader);
        }
    }
}
