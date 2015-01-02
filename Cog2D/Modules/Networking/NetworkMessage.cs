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
        private static Dictionary<ushort, Action<BinaryReader, BinaryWriter>> messageReceiverCache;
        private static Dictionary<ushort, Action<Object, BinaryReader, IStringCacher>> messageReaderCache;
        private static Dictionary<ushort, Func<NetworkMessage>> eventCreators;
        private static Dictionary<Type, ushort> typeIds;
        private static List<Type> types;
        public static byte[] NetworkingHash;

        [NetworkIgnore()]
        private CogClient _client;
        public CogClient Client { get { return _client; } internal set { _client = value; } }

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

        internal static Type GetType(ushort id)
        {
            return types[(int)id - 1];
        }

        internal static void InitializeCache()
        {
            nextId = 1;

            typeIds = new Dictionary<Type, ushort>();
            eventCreators = new Dictionary<ushort, Func<NetworkMessage>>();
            messageWriterCache = new Dictionary<ushort, Action<object, BinaryWriter, IStringCacher>>();
            messageReceiverCache = new Dictionary<ushort, Action<BinaryReader, BinaryWriter>>();
            messageReaderCache = new Dictionary<ushort, Action<object, BinaryReader, IStringCacher>>();
            types = new List<Type>();
        }

        internal static void CreateCache(Type type)
        {
            if (type.ContainsGenericParameters)
                throw new Exception("Types that derive from NetworkMessage may not be generic!");

            Action<Object, BinaryWriter, IStringCacher> messageWriter = null;
            Action<Object, BinaryReader, IStringCacher> messageReader = null;
            Action<BinaryReader, BinaryWriter> messageReceiver = null;
            // Iterate through all fields contained within the type alphabetically
            foreach (var currentField in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).OrderBy(o => o.Name))
            {
                if (currentField.GetCustomAttributes(typeof(NetworkIgnoreAttribute), true).Length == 0)
                {
                    // Create a new local variable to access the current field, this variable will be accessed later as messages are written to the stream
                    var field = currentField;
                    var fieldType = field.FieldType;

                    // Find the specific type's writer/reader helper
                    if (fieldType == typeof(string))
                    {
                        var properties = (StringPropertiesAttribute)field.GetCustomAttributes(typeof(StringPropertiesAttribute), true).FirstOrDefault();
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

                                messageReceiver += (r, w) =>
                                {
                                    var size = r.ReadUInt16();
                                    w.Write((UInt16)size);
                                    if (size > 0)
                                        w.Write(r.ReadBytes(size));
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

                                messageReceiver += (r, w) =>
                                {
                                    w.Write(r.ReadUInt16());
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
                            messageReceiver += writer.GenericCopy;
                            messageReader += (o, r, sc) => field.SetValue(o, writer.GenericRead(r));
                        }
                        else if (fieldType.IsEnum)
                        {
                            messageWriter += (o, w, sc) => w.Write((UInt16)field.GetValue(o));
                            messageReceiver += (r, w) => w.Write((UInt16)r.ReadUInt16());
                            messageReader += (o, r, sc) => field.SetValue(o, r.ReadUInt16());
                        }
                        else
                            throw new Exception(string.Format("Network Event type {0} contains a type without a registered type handler: {1}\r\nEither register a custom type handler through NetworkEvent.RegisterCustomType<T>(writer, reader) or add a NetworkIgnore-attribute to the field.", type.FullName, field.Name));
                    }
                }
            }

            // Register the collection of write/read functions for this type
            messageReaderCache[nextId] = messageReader;
            messageReceiverCache[nextId] = messageReceiver;
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
                    if (currentField.GetCustomAttributes(typeof(NetworkIgnoreAttribute), true).FirstOrDefault() == null)
                    {
                        // Create a new local variable to access the current field, this variable will be accessed later as messages are written to the stream
                        var field = currentField;
                        var fieldType = field.FieldType;

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
        internal static void WriteToSocket<T>(T message, CogClient socket)
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

                    if (!socket.IsDisconnected)
                    {
                        try
                        {
                            socket.Writer.Write(stream.ToArray());
                        }
                        catch (IOException e)
                        {
                            Debug.Error("Disconnecting client {0}: {1}", socket.IpAddress, e.Message);
                            socket.Disconnect();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reads an event of the given type from the given BinaryReader
        /// </summary>
        internal static byte[] ReadMessageData(UInt16 type, CogClient socket, BinaryReader reader)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    var messageReceiver = messageReceiverCache[type];
                    if (messageReceiver != null)
                        messageReceiver(reader, writer);

                    return stream.ToArray();
                }
            }
        }

        internal static NetworkMessage ReadMessage(UInt16 typeId, byte[] data, CogClient client)
        {
            using (MemoryStream stream = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                var type = GetType(typeId);
                NetworkMessage ev = eventCreators[typeId]();
                messageReaderCache[typeId](ev, reader, client);
                ev.Client = client;
                return ev;
            }
        }
    }
}
