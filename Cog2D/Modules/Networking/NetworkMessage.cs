using Cog.Modules.Content;
using Cog.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Networking
{
    public abstract class NetworkMessage
    {
        private static ushort nextId;
        private static Dictionary<ushort, Action<Object, BinaryWriter, IStringCacher>> messageWriterCache;
        private static Dictionary<ushort, Action<Object, BinaryReader, IStringCacher>> messageReaderCache;
        private static Dictionary<ushort, Func<NetworkMessage>> eventCreators;
        private static Dictionary<Type, ushort> typeIds;
        private static List<Type> types;
        public static byte[] NetworkingHash;

        [NetworkIgnore()]
        private TcpSocket _sender;
        /// <summary>
        /// The TcpSocket which sent us this message
        /// </summary>
        public TcpSocket Sender { get { return _sender; } }
        public CogClient Client { get { return _sender as CogClient; } }

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

        internal static void InitializeCache()
        {
            nextId = 1;

            typeIds = new Dictionary<Type, ushort>();
            eventCreators = new Dictionary<ushort, Func<NetworkMessage>>();
            messageWriterCache = new Dictionary<ushort, Action<object, BinaryWriter, IStringCacher>>();
            messageReaderCache = new Dictionary<ushort, Action<object, BinaryReader, IStringCacher>>();
            types = new List<Type>();
        }

        internal static void CreateCache(Type type)
        {
            if (type.ContainsGenericParameters)
                throw new Exception("Types that derive from NetworkMessage may not be generic!");

            Action<Object, BinaryWriter, IStringCacher> messageWriter = null;
            Action<Object, BinaryReader, IStringCacher> messageReader = null;
            // Iterate through all fields contained within the type alphabetically
            foreach (var currentField in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).OrderBy(o => o.Name))
            {
                if (currentField.GetCustomAttribute<NetworkIgnoreAttribute>() == null)
                {
                    // Create a new local variable to access the current field, this variable will be accessed later as messages are written to the stream
                    var field = currentField;
                    var fieldType = field.FieldType;

                    // Find the specific type's writer/reader helper
                    if (fieldType == typeof(string))
                    {
                        var properties = field.GetCustomAttribute<StringPropertiesAttribute>();
                        if (properties == null)
                            properties = new StringPropertiesAttribute(StringSendType.DynamicUShort);
                        switch (properties.Type)
                        {
                            case StringSendType.DynamicUShort:
                                messageWriter += (o, w, sc) =>
                                {
                                    string value = (string)field.GetValue(o);
                                    if (value == null || value.Length == 0)
                                        w.Write((UInt16)0);
                                    else
                                    {
                                        var data = Encoding.UTF8.GetBytes(value);
                                        w.Write((UInt16)data.Length);
                                        w.Write(data);
                                    }
                                };

                                messageReader += (o, r, sc) =>
                                {
                                    string value = "";
                                    int size = (int)r.ReadUInt16();
                                    if (size > 0)
                                        value = Encoding.UTF8.GetString(r.ReadBytes(size));
                                    field.SetValue(o, value);
                                };
                                break;

                            case StringSendType.Cached:
                                messageWriter += (o, w, sc) =>
                                {
                                    var value = (string)field.GetValue(o);
                                    w.Write((ushort)sc.GetIdFromString(value));
                                };

                                messageReader += (o, r, sc) =>
                                {
                                    ushort id = r.ReadUInt16();
                                    field.SetValue(o, sc.GetStringFromId(id));
                                };
                                break;

                            default:
                                throw new NotImplementedException("String Send Type \"" + properties.Type.ToString() + "\" is not implemented!");
                        }
                    }
                    else
                    {
                        ITypeWriter writer = TypeSerializer.GetTypeWriter(fieldType);
                        if (writer != null)
                        {
                            // Add write/read operations to the type handler
                            messageWriter += (o, w, sc) => writer.GenericWrite(field.GetValue(o), w);
                            messageReader += (o, r, sc) => field.SetValue(o, writer.GenericRead(r));
                        }
                        else if (fieldType.IsEnum)
                        {
                            messageWriter += (o, w, sc) => w.Write((UInt16)field.GetValue(o));
                            messageReader += (o, r, sc) => field.SetValue(o, r.ReadUInt16());
                        }
                        else if (typeof(ISerializable).IsAssignableFrom(fieldType))
                        {
                            messageWriter += (o, w, sc) => { ((ISerializable)field.GetValue(o)).Serialize(w); };
                            messageReader += (o, r, sc) => { var v = (ISerializable)FormatterServices.GetUninitializedObject(fieldType); v.Deserialize(r); field.SetValue(o, v); };
                        }
                        else
                            throw new Exception(string.Format("Network Event type {0} contains a type without a registered type handler: {1}\r\nEither register a custom type handler through NetworkEvent.RegisterCustomType<T>(writer, reader) or add a NetworkIgnore-attribute to the field.", type.FullName, field.Name));
                    }
                }
            }

            // Register the collection of write/read functions for this type
            messageReaderCache[nextId] = messageReader;
            messageWriterCache[nextId] = messageWriter;
            // Create an anonymous function that creates an instance of the specified type without invoking it's constructor
            eventCreators[nextId] = () => (NetworkMessage)FormatterServices.GetUninitializedObject(type);
            // Register the ID to this type
            typeIds[type] = nextId;
            types.Add(type);

            nextId++;
        }

        internal static string GetNetworkDescriber()
        {
            // Contains a string describing the networking classes
            StringBuilder networkingDescriber = new StringBuilder();
            networkingDescriber.Append("- NETWORK MESSAGES\n");

            foreach (var type in types)
            {
                networkingDescriber.Append(type.FullName);
                networkingDescriber.Append('{');

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
                    }
                }
                networkingDescriber.Append('}');
                networkingDescriber.Append('\n');
            }

            return networkingDescriber.ToString();
        }
        
        /// <summary>
        /// Creates a byte array portraying the given NetworkEvent, including event ID and size
        /// </summary>
        internal static void WriteToSocket<T>(T message, TcpSocket socket)
            where T : NetworkMessage
        {
            var id = GetId<T>();

            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write((UInt16)id);

                    var messageWriter = messageWriterCache[id];
                    if (messageWriter != null)
                        messageWriter((Object)message, writer, socket);
                }

                socket.Writer.Write(stream.ToArray());
            }
        }

        /// <summary>
        /// Reads an event of the given type from the given BinaryReader
        /// </summary>
        internal static NetworkMessage ReadMessage(TcpSocket sender, TcpSocket socket)
        {
            UInt16 type = socket.Reader.ReadUInt16();

            NetworkMessage ev = eventCreators[type]();
            var messageReader = messageReaderCache[type];
            if (messageReader != null)
                messageReader((Object)ev, socket.Reader, socket);
            ev._sender = sender;
            return ev;
        }
    }
}
