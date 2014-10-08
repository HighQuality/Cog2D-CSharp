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

            typeWriters[typeof(T[])] = new TypeWriter<T[]>((v, w) =>
            {
                w.Write((UInt16)v.Length);
                for (int i = 0; i < v.Length; i++)
                    writer(v[i], w);
            }, (r) =>
            {
                var length = r.ReadUInt16();
                T[] arr = new T[length];
                for (int i = 0; i < arr.Length; i++)
                    arr[i] = reader(r);
                return arr;
            });

            typeWriters[typeof(T[][])] = new TypeWriter<T[][]>((v, w) =>
            {
                w.Write((UInt16)v.Length);
                for (int i = 0; i < v.Length; i++)
                {
                    w.Write((UInt16)v[i].Length);
                    for (int j=0; j<v[i].Length; j++)
                    {
                        writer(v[i][j], w);
                    }
                }
            }, (r) =>
            {
                var length = r.ReadUInt16();
                T[][] arr = new T[length][];
                for (int i = 0; i < arr.Length; i++)
                {
                    var innerArr = new T[r.ReadUInt16()];
                    arr[i] = innerArr;
                    for (int j = 0; j < innerArr.Length; j++)
                        innerArr[j] = reader(r);
                }
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

        public static void Serialize(Object obj, BinaryWriter writer)
        {
            var type = obj.GetType();
            var typeWriter = GetTypeWriter(type);
            if (typeWriter == null)
            {
                /*if (type.IsArray)
                {
                    var current = type;
                    Array array = (Array)obj;
                    
                    // Find the base type
                    Type baseType = type;
                    do
                    {
                        baseType = baseType.GetElementType();
                    }
                    while (baseType.IsArray);

                    typeWriter = GetTypeWriter(baseType);

                    if (typeWriter == null)
                        throw new Exception("Type \"" + baseType.FullName + "\" does not have a type serializer!");
                    Array current = array;
                    do
                    {

                        current = current.GetElementType();
                    }
                    while (current.IsArray);
                }
                else*/
                throw new Exception("No type writer for \"" + obj.GetType().FullName + "\" exists!");
            }
            else
            {
                typeWriter.GenericWrite(obj, writer);
            }
        }

        public static TypeWriter<T> GetTypeWriter<T>()
        {
            return (TypeWriter<T>)GetTypeWriter(typeof(T));
        }
    }
}
