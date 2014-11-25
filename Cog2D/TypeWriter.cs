using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog
{
    public interface ITypeWriter
    {
        void GenericCopy(BinaryReader reader, BinaryWriter writer);
        object GenericRead(BinaryReader reader);
        void GenericWrite(Object value, BinaryWriter writer);
    }

    public class TypeWriter<T> : ITypeWriter
    {
        public Action<BinaryReader, BinaryWriter> Copy { get; private set; }
        public Action<T, BinaryWriter> Writer { get; private set; }
        public Func<BinaryReader, T> Reader { get; private set; }

        public TypeWriter(Action<BinaryReader, BinaryWriter> copy, Action<T, BinaryWriter> writer, Func<BinaryReader, T> reader)
        {
            this.Copy = copy;
            this.Writer = writer;
            this.Reader = reader;
        }

        public object GenericRead(BinaryReader reader)
        {
            return Reader(reader);
        }
        public void GenericWrite(Object value, BinaryWriter writer)
        {
            Writer((T)value, writer);
        }
        public void GenericCopy(BinaryReader reader, BinaryWriter writer)
        {
            Copy(reader, writer);
        }
    }
}
