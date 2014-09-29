using Square.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.Networking
{
    public class NetworkEvent : EventParameters
    {
        private static ushort nextId;
        private static Dictionary<ushort, Action<Object, BinaryReader>> eventReaderCache;
        private static Dictionary<ushort, Action<Object, BinaryWriter>> eventWriterCache;
        private static Dictionary<ushort, Func<NetworkEvent>> eventCreators;
        private static Dictionary<Type, ITypeWriter> typeWriters;
        private static Dictionary<Type, ushort> typeIds;

        public NetworkEvent()
            : base(null)
        {

        }

        public static void RegisterCustomType<T>(Action<T, BinaryWriter> writer, Func<BinaryReader, T> reader)
        {
            if (typeWriters == null)
                typeWriters = new Dictionary<Type, ITypeWriter>();
            typeWriters[typeof(T)] = new TypeWriter<T>(writer, reader);
        }

        internal static ushort GetId<T>()
            where T : NetworkEvent
        {
            return typeIds[typeof(T)];
        }

        internal static void BuildCache(IEnumerable<Assembly> assemblies)
        {
            if (nextId != 0)
                throw new Exception("NetworkEvent-cache has already been built!");
            nextId = 1;

            typeIds = new Dictionary<Type, ushort>();
            eventCreators = new Dictionary<ushort, Func<NetworkEvent>>();
            eventReaderCache = new Dictionary<ushort, Action<object, BinaryReader>>();
            eventWriterCache = new Dictionary<ushort, Action<object, BinaryWriter>>();

            List<Type> types = new List<Type>();
            foreach (var assembly in assemblies)
                foreach (var type in assembly.GetTypes())
                    types.Add(type);

            foreach (var currentType in types.OrderBy(o => o.FullName))
            {
                if (typeof(NetworkEvent).IsAssignableFrom(currentType))
                {
                    var type = currentType;

                    Console.WriteLine("Network Id {0} = {1}", nextId, type.FullName);

                    Action<Object, BinaryReader> messageReader = null;
                    Action<Object, BinaryWriter> messageWriter = null;
                    foreach (var currentField in type.GetFields().OrderBy(o => o.Name))
                    {
                        if (currentField.GetCustomAttribute<NetworkIgnoreAttribute>() == null)
                        {
                            var field = currentField;
                            var fieldType = field.FieldType;
                            Console.WriteLine("\t{0} {1}", fieldType.Name, field.Name);
                            ITypeWriter writer;
                            if (typeWriters.TryGetValue(fieldType, out writer))
                            {
                                messageReader += (o, r) => field.SetValue(o, writer.GenericRead(r));
                                messageWriter += (o, w) => writer.GenericWrite(field.GetValue(o), w);
                            }
                            else
                                throw new Exception(string.Format("Network Event type {0} contains a type without a registered type handler: {1}\r\nEither register a custom type handler through NetworkEvent.RegisterCustomType<T>(writer, reader) or add a NetworkIgnore-attribute to the field.", type.FullName, field.Name));
                        }
                    }


                    eventReaderCache[nextId] = messageReader;
                    eventWriterCache[nextId] = messageWriter;
                    eventCreators[nextId] = () => (NetworkEvent)FormatterServices.GetUninitializedObject(type);
                    typeIds[type] = nextId;

                    nextId++;
                }
            }
        }

        /// <summary>
        /// Creates a byte array portraying the given NetworkEvent, including event ID and size
        /// </summary>
        internal static byte[] ToByteArray<T>(T data)
            where T : NetworkEvent
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    var id = GetId<T>();
                    writer.Write((UInt32)0);
                    writer.Write((UInt16)id);
                    eventWriterCache[id]((Object)data, writer);
                    long size = stream.Position - sizeof(UInt32);
                    stream.Seek(0, SeekOrigin.Begin);
                    writer.Write((UInt32)size);
                }
                return stream.ToArray();
            }
        }

        internal static NetworkEvent ReadEvent(UInt16 type, BinaryReader reader)
        {
            NetworkEvent ev = eventCreators[type]();
            eventReaderCache[type]((Object)ev, reader);
            return ev;
        }
    }
}
