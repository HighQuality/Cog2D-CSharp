using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square
{
    public static class TypeSerializer
    {
        private static Dictionary<Type, ITypeWriter> typeWriters;

        /// <summary>
        /// Registers a type to be allowed within a NetworkEvent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="writer"></param>
        /// <param name="reader"></param>
        public static void Register<T>(Action<T, BinaryWriter> writer, Func<BinaryReader, T> reader)
        {
            if (typeWriters == null)
                typeWriters = new Dictionary<Type, ITypeWriter>();
            typeWriters[typeof(T)] = new TypeWriter<T>(writer, reader);
        }

        public static bool SerializerExists<T>()
        {
            return typeWriters.ContainsKey(typeof(T));
        }

        public static ITypeWriter GetTypeWriter(Type type)
        {
            ITypeWriter writer;
            typeWriters.TryGetValue(type, out writer);
            return writer;
        }

        public static TypeWriter<T> GetTypeWriter<T>()
        {
            ITypeWriter writer;
            typeWriters.TryGetValue(typeof(T), out writer);
            return (TypeWriter<T>)writer;
        }
    }
}
