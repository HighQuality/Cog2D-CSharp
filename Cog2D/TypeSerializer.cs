using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog
{
    public static class TypeSerializer
    {
        private static Dictionary<Type, ITypeWriter> typeWriters;

        /// <summary>
        /// Registers methods to serialize a type and an array-subtype 
        /// </summary>
        public static void Register<T>(Action<T, BinaryWriter> writer, Func<BinaryReader, T> reader)
        {
            if (typeof(T).IsArray)
                throw new Exception("Array-variants are automatically implemented and may not be implemented manually!");
            if (typeWriters == null)
                typeWriters = new Dictionary<Type, ITypeWriter>();

            typeWriters[typeof(T)] = new TypeWriter<T>(writer, reader);

            // Register array handle
            typeWriters[typeof(T[])] = new TypeWriter<T[]>((v, w) =>
            {
                // Writer
                if (v == null)
                {
                    w.Write((UInt16)0);
                }
                else
                {
                    w.Write((UInt16)v.Length);
                    for (int i = 0; i < v.Length; i++)
                        writer(v[i], w);
                }
            }, r =>
            {
                // Reader
                UInt16 length = r.ReadUInt16();
                T[] arr = new T[length];
                for (int i = 0; i < length; i++)
                    arr[i] = reader(r);
                return arr;
            });
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
