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
    public abstract class NetworkMessage
    {
        private static ushort nextId;
        private static Dictionary<ushort, Action<Object, BinaryReader>> eventReaderCache;
        private static Dictionary<ushort, Action<Object, BinaryWriter>> eventWriterCache;
        private static Dictionary<ushort, Func<NetworkMessage>> eventCreators;
        private static Dictionary<Type, ushort> typeIds;
        public static byte[] NetworkingHash;

        [NetworkIgnore()]
        public Int32 MessageSize;
        [NetworkIgnore()]
        private TcpSocket _sender;
        /// <summary>
        /// The TcpSocket which sent us this message
        /// </summary>
        public TcpSocket Sender { get { return _sender; } }
        public SquareClient Client { get { return _sender as SquareClient; } }

        public NetworkMessage()
        {
        }

        /// <summary>
        /// Invoked when a message of the derived type has been received
        /// </summary>
        /// <returns></returns>
        public abstract void Received();
        
        /// <summary>
        /// Gets the identifier
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal static ushort GetId<T>()
            where T : NetworkMessage
        {
            return typeIds[typeof(T)];
        }

        internal static void BuildCache(IEnumerable<Assembly> assemblies)
        {
            if (nextId != 0)
                throw new Exception("NetworkEvent-cache has already been built!");
            nextId = 1;

            // Contains a string describing the networking classes
            StringBuilder networkingDescriber = new StringBuilder();

            typeIds = new Dictionary<Type, ushort>();
            eventCreators = new Dictionary<ushort, Func<NetworkMessage>>();
            eventReaderCache = new Dictionary<ushort, Action<object, BinaryReader>>();
            eventWriterCache = new Dictionary<ushort, Action<object, BinaryWriter>>();

            // Build a list of all classes which derive from NetworkEvent
            List<Type> types = new List<Type>();
            foreach (var assembly in assemblies)
                foreach (var type in assembly.GetTypes())
                    if (typeof(NetworkMessage).IsAssignableFrom(type))
                        types.Add(type);

            // Iterate through them alphabetically
            foreach (var type in types.OrderBy(o => o.FullName))
            {
                Debug.Info("Network Id {0} = {1}", nextId, type.FullName);

                networkingDescriber.Append(type.FullName);
                networkingDescriber.Append('{');

                Action<Object, BinaryReader> messageReader = null;
                Action<Object, BinaryWriter> messageWriter = null;
                // Iterate through all fields contained within the type alphabetically
                foreach (var currentField in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).OrderBy(o => o.Name))
                {
                    if (currentField.GetCustomAttribute<NetworkIgnoreAttribute>() == null)
                    {
                        // Create a new local variable to access the current field, this variable will be accessed later as messages are written to the stream
                        var field = currentField;
                        var fieldType = field.FieldType;

                        Debug.Info("\t{0} {1}", fieldType.Name, field.Name);
                        networkingDescriber.Append(fieldType.Name);
                        networkingDescriber.Append(' ');
                        networkingDescriber.Append(field.Name);
                        networkingDescriber.Append(';');

                        // Find the specific type's writer/reader helper
                        ITypeWriter writer = TypeSerializer.GetTypeWriter(fieldType);
                        if (writer != null)
                        {
                            // Add write/read operations to the type handler
                            messageReader += (o, r) => field.SetValue(o, writer.GenericRead(r));
                            messageWriter += (o, w) => writer.GenericWrite(field.GetValue(o), w);
                        }
                        else if (fieldType.IsEnum)
                        {
                            messageReader += (o, r) => field.SetValue(o, r.ReadUInt16());
                            messageWriter += (o, w) => w.Write((UInt16)field.GetValue(o));
                        }
                        else
                            throw new Exception(string.Format("Network Event type {0} contains a type without a registered type handler: {1}\r\nEither register a custom type handler through NetworkEvent.RegisterCustomType<T>(writer, reader) or add a NetworkIgnore-attribute to the field.", type.FullName, field.Name));
                    }
                }
                networkingDescriber.Append('}');
                networkingDescriber.Append('\n');

                // Register the collection of write/read functions for this type
                eventReaderCache[nextId] = messageReader;
                eventWriterCache[nextId] = messageWriter;
                // Create an anonymous function that creates an instance of the specified type without invoking it's constructor
                eventCreators[nextId] = () => (NetworkMessage)FormatterServices.GetUninitializedObject(type);
                // Register the ID to this type
                typeIds[type] = nextId;

                nextId++;
            }

            Console.WriteLine(networkingDescriber.ToString());
            // Generates a SHA256-hash from the string describing the networking classes
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                NetworkingHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(networkingDescriber.ToString()));
                Console.WriteLine(BitConverter.ToString(NetworkingHash).Replace("-", ""));
            }
        }
        
        /// <summary>
        /// Creates a byte array portraying the given NetworkEvent, including event ID and size
        /// </summary>
        internal static byte[] ToByteArray<T>(T data)
            where T : NetworkMessage
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    var id = GetId<T>();
                    writer.Write((UInt32)0);

                    writer.Write((UInt16)id);
                    
                    var messageWriter = eventWriterCache[id];
                    if (messageWriter != null)
                        messageWriter((Object)data, writer);

                    long size = stream.Position - sizeof(UInt32);
                    stream.Seek(0, SeekOrigin.Begin);
                    writer.Write((UInt32)size);
                }
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Reads an event of the given type from the given BinaryReader
        /// </summary>
        internal static NetworkMessage ReadEvent(TcpSocket sender, UInt16 type, BinaryReader reader)
        {
            NetworkMessage ev = eventCreators[type]();
            long initialPosition = reader.BaseStream.Position;
            var messageReader = eventReaderCache[type];
            if (messageReader != null)
                messageReader((Object)ev, reader);
            ev.MessageSize = (int)(reader.BaseStream.Position - initialPosition);
            typeof(NetworkMessage).GetField("_sender", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(ev, sender);
            return ev;
        }
    }
}
