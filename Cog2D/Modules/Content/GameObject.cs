using Cog.Modules.EventHost;
using Cog.Modules.Networking;
using Cog.Modules.Renderer;
using Cog.Modules.Resources;
using Cog.Scenes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public abstract class GameObject : IBoundingBox
    {
        private static Dictionary<Type, UInt16> objectsDictionary;
        private static Dictionary<Type, FieldInfo[]> objectsSynchronizedProperties;
        private static Dictionary<FieldInfo, UInt16> synchronizedIds;
        private static List<Type> objectsArray;
        private static UInt16 nextObjectId;

        internal InitializationData InitializationData;

        public string ObjectName;
        private GameObject _parent;
        public GameObject Parent
        {
            get { return _parent; }
            // PARENT IS ALSO SET THROUGH InitialSetParent FUNCTION WITHOUT INVOKING THIS FUNCTION
            set
            {
                if (_parent != null)
                {
                    _parent.children.Remove(this);

                    // If we had a parent but don't anymore
                    if (value == null)
                    {
                        // We're now a base object
                        Scene.BaseObjects.AddLast(this);
                        HashSet<GameObject> objectSet;
                        DrawCell cell;
                        cell.X = (int)LocalCoord.X / DrawCell.DrawCellSize;
                        cell.Y = (int)LocalCoord.Y / DrawCell.DrawCellSize;
                        if (!Scene.DrawCells.TryGetValue(cell, out objectSet))
                        {
                            objectSet = new HashSet<GameObject>();
                            Scene.DrawCells.Add(cell, objectSet);
                        }
                        objectSet.Add(this);
                    }
                }
                else if (value != null)
                {
                    // If we got a parent now get rid of our base objects and draw entries
                    var it = Scene.BaseObjects.Last;
                    while (it != null)
                    {
                        if (it.Value == this)
                        {
                            Scene.BaseObjects.Remove(it);
                            break;
                        }
                        it = it.Previous;
                    }

                    HashSet<GameObject> objectSet;
                    DrawCell cell;
                    cell.X = (int)LocalCoord.X / DrawCell.DrawCellSize;
                    cell.Y = (int)LocalCoord.Y / DrawCell.DrawCellSize;
                    if (Scene.DrawCells.TryGetValue(cell, out objectSet))
                    {
                        objectSet.Remove(this);
                        if (objectSet.Count == 0)
                            Scene.DrawCells.Remove(cell);
                    }
                }

                _parent = value;
                if (_parent != null)
                {
                    if (_parent.children == null)
                        _parent.children = new List<GameObject>();
                    _parent.children.Add(this);
                }
            }
        }

        private List<GameObject> children;

        public HashSet<CogClient> SubscribedClients = new HashSet<CogClient>();

        private Vector2 _localCoord;
        public Vector2 LocalCoord
        {
            get { return _localCoord; }
            set
            {
                if (Parent == null)
                {
                    DrawCell currentCell,
                        newCell;
                    currentCell.X = ((int)_localCoord.X / DrawCell.DrawCellSize);
                    currentCell.Y = ((int)_localCoord.Y / DrawCell.DrawCellSize);
                    newCell.X = ((int)value.X / DrawCell.DrawCellSize);
                    newCell.Y = ((int)value.Y / DrawCell.DrawCellSize);

                    if (newCell.X != currentCell.X || newCell.Y != currentCell.Y)
                    {
                        HashSet<GameObject> objectSet;
                        // Remove our current entry
                        if (Scene.DrawCells.TryGetValue(currentCell, out objectSet))
                        {
                            objectSet.Remove(this);
                            if (objectSet.Count == 0)
                                Scene.DrawCells.Remove(currentCell);
                        }
                        // Add our new entry
                        if (!Scene.DrawCells.TryGetValue(newCell, out objectSet))
                        {
                            objectSet = new HashSet<GameObject>();
                            Scene.DrawCells.Add(newCell, objectSet);
                        }
                        objectSet.Add(this);
                    }
                }
                _localCoord = value;
            }
        }
        public virtual Vector2 WorldCoord
        {
            get
            {
                if (Parent == null)
                    return LocalCoord;
                return Parent.WorldCoord + (LocalCoord * Parent.WorldScale).Rotate(Parent.WorldRotation);
            }
            set
            {
                if (Parent == null)
                    LocalCoord = value;
                else
                    LocalCoord = (value - Parent.WorldCoord).Rotate(-Parent.WorldRotation) / Parent.WorldScale;
            }
        }

        public Angle LocalRotation { get; set; }
        public virtual Angle WorldRotation
        {
            get
            {
                if (Parent == null)
                    return LocalRotation;
                return Parent.WorldRotation + LocalRotation;
            }
            set
            {
                if (Parent == null)
                    LocalRotation = value;
                else
                    LocalRotation = value - Parent.WorldRotation;
            }
        }

        private Vector2 _localScale = Vector2.One;
        public Vector2 LocalScale { get { return _localScale; } set { _localScale = value; } }
        public virtual Vector2 WorldScale
        {
            get
            {
                if (Parent == null)
                    return LocalScale;
                return Parent.WorldScale * LocalScale;
            }
            set
            {
                if (Parent == null)
                    LocalScale = value;
                else
                    LocalScale = value / Parent.WorldScale;
            }
        }

        public ResourceCollection Resources { get; private set; }

        public Rectangle BoundingBox { get { return new Rectangle(WorldCoord, Size); } }
        public Vector2 Size { get; set; }
        public Scene Scene { get; internal set; }
        public bool DoRemove { get; private set; }
        public CogClient Owner { get; internal set; }
        public long Id { get; internal set; }
        public bool IsGlobal { get { return Id > 0; } }
        public bool IsLocal { get { return Id < 0; } }
        private List<IEventListener> registeredEvents;

        internal List<Action<DrawEvent, DrawTransformation>> OnDraw;

        public GameObject()
        {
            if (InitializationData == null)
                throw new InvalidOperationException("GameObject.InitalizationData was never set! Did you create the object through one of the Scene creation methods?");

            var type = GetType();
            
            Resources = Engine.ResourceHost.GetResourceCollection(type);
            ObjectName = type.Name;

            var fields = objectsSynchronizedProperties[type];

            for (int i = 1; i < fields.Length; i++)
            {
                var field = fields[i];

                ISynchronized member = (ISynchronized)field.GetValue(this);
                member.BaseObject = this;
                member.SynchronizationId = synchronizedIds[field];
                member.TypeWriter = TypeSerializer.GetTypeWriter(member.GetType().GenericTypeArguments[0]);

                if (InitializationData.SynchronizedValues != null)
                    member.ForceSet(InitializationData.SynchronizedValues[i - 1]);

                field.SetValue(this, member);
            }

            // Initialization has finished, get rid of data that is no longer necessary
            InitializationData = null;
        }

        public virtual void Initialize()
        {
        }
        
        public bool KeyIsDown(Keyboard.Key key)
        {
            if (Engine.IsClient)
                return Engine.Window.IsKeyDown(key);
            else if (Owner != null)
                return Owner.IsKeyDown(key);
            return false;
        }
                
        public void Remove()
        {
            if (registeredEvents != null)
            {
                for (int i = registeredEvents.Count - 1; i >= 0; i--)
                    registeredEvents[i].Cancel();
                registeredEvents = null;
            }

            DoRemove = true;
        }
        
        public EventListener<T> RegisterEvent<T>(int priority, Action<T> action)
            where T : EventParameters
        {
            return RegisterEvent<T>(null, priority, action);
        }

        public EventListener<T> RegisterEvent<T>(Object uniqueIdentifier, int priority, Action<T> action)
            where T : EventParameters
        {
            var listener = Scene.EventModule.RegisterEvent(uniqueIdentifier, priority, action);
            Scene.AddEventStrength<T>(listener);
            if (registeredEvents == null)
                registeredEvents = new List<IEventListener>();
            registeredEvents.Add(listener);
            return listener;
        }

        public void Send<T>(T message)
            where T : NetworkMessage
        {
            foreach (var client in Scene.EnumerateSubscribedClients())
                client.Send(message);
        }
        
        /// <summary>
        /// Sets the parent witout triggering Parent's setter.
        /// Only to be used internally by the engine when the object is first created
        /// </summary>
        internal void InitialSetParent(GameObject parent)
        {
            this._parent = parent;
            
            if (parent != null)
            {
                if (parent.children == null)
                    parent.children = new List<GameObject>();
                parent.children.Add(this);
            }
        }

        internal CreateObjectMessage CreateCreationMessage()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    Serialize(writer);
                    return new CreateObjectMessage(stream.ToArray());
                }
            }
        }

        internal void Serialize(BinaryWriter writer)
        {
            // Id
            var objectId = objectsDictionary[GetType()];
            writer.Write((UInt16)objectId);
            writer.Write((Int64)Id);

            // Transformation
            writer.Write((float)LocalCoord.X);
            writer.Write((float)LocalCoord.Y);
            writer.Write((float)LocalRotation.Degree);
            writer.Write((float)LocalScale.X);
            writer.Write((float)LocalScale.Y);

            // 
            var fields = objectsSynchronizedProperties[GetType()];
            for (int i = 1; i < fields.Length; i++)
            {
                var serializer = TypeSerializer.GetTypeWriter(fields[i].FieldType.GenericTypeArguments[0]);
                ISynchronized value = (ISynchronized)fields[i].GetValue(this);
                serializer.GenericWrite(value.GenericGet(), writer);
            }

            // TODO: User Data
            /*
             var userData = obj.WriteUserData();
             writer.Write((UInt32)userData.Length);
             writer.Write((byte[])userData);
             */

            // Children
            if (children != null)
            {
                writer.Write((UInt16)children.Where(o => o.IsGlobal).Count());
                for (int i = 0; i < children.Count; i++)
                    if (children[i].IsGlobal)
                        children[i].Serialize(writer);
            }
            else
                writer.Write((UInt16)0);
        }

        internal static GameObject Deserialize(Scene scene, GameObject parent, BinaryReader reader)
        {
            var objects = DeserializeUninitialized(scene, parent, reader).ToArray();

            // Parent -> Children
            for (int i = 0; i < objects.Length; i++)
                scene.InitializeObject(objects[i]);
            for (int i = 0; i < objects.Length; i++)
                objects[i].Initialize();

            // Return main object
            return objects[0];
        }

        internal static IEnumerable<GameObject> DeserializeUninitialized(Scene scene, GameObject parent, BinaryReader reader)
        {
            // Id
            ushort objectId = reader.ReadUInt16();
            var id = reader.ReadInt64();
            GameObject obj = scene.CreateUninitializedObject(objectsArray[(int)objectId], parent);
            Engine.AssignId(obj, id);

            // Transformation
            Vector2 coord,
                scale;
            float rotDegree;

            coord.X = reader.ReadSingle();
            coord.Y = reader.ReadSingle();
            rotDegree = reader.ReadSingle();
            scale.X = reader.ReadSingle();
            scale.Y = reader.ReadSingle();

            obj.LocalCoord = coord;
            obj.LocalRotation = Angle.FromDegree(rotDegree);
            obj.LocalScale = scale;

            // Read properties
            var fields = objectsSynchronizedProperties[obj.GetType()];
            object[] synchronizedValues = new object[fields.Length - 1];
            for (int i = 1; i < fields.Length; i++)
            {
                var readType = fields[i].FieldType.GenericTypeArguments[0];
                var serializer = TypeSerializer.GetTypeWriter(readType);
                synchronizedValues[i - 1] = serializer.GenericRead(reader);
            }

            // Assign synchronized values to be set by GameObject's constructor
            obj.InitializationData.SynchronizedValues = synchronizedValues;

            // TODO: User data
            /*var userSize = reader.ReadUInt32();
            var userData = reader.ReadBytes((int)userSize);*/

            // Yield this object back to the caller, who initializes them after the hierarcy has been created 
            yield return obj;

            // Children
            var childCount = reader.ReadUInt16();
            for (int i = 0; i < childCount; i++)
                foreach (var child in DeserializeUninitialized(scene, obj, reader))
                    yield return child;
        }

        internal static GameObject CreateFromData(Scene scene, byte[] objectData)
        {
            using (MemoryStream stream = new MemoryStream(objectData))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    return Deserialize(scene, null, reader);
                }
            }
        }

        internal void Draw(DrawEvent ev, DrawTransformation transform)
        {
            if (children != null)
            {
                transform.WorldCoord += (LocalCoord * transform.ParentWorldScale).Rotate(transform.ParentWorldRotation);
                transform.WorldRotation += LocalRotation;
                transform.WorldScale *= LocalScale;

                if (OnDraw != null)
                    for (int i = 0; i < OnDraw.Count; i++)
                        OnDraw[i](ev, transform);

                transform.ParentWorldCoord = transform.WorldCoord;
                transform.ParentWorldRotation = transform.WorldRotation;
                transform.ParentWorldScale = transform.WorldScale;

                int c = children.Count;
                for (int i = 0; i < c; i++)
                    children[i].Draw(ev, transform);
            }
            else if (OnDraw != null)
            {
                transform.WorldCoord += (LocalCoord * transform.ParentWorldScale).Rotate(transform.ParentWorldRotation);
                transform.WorldRotation += LocalRotation;
                transform.WorldScale *= LocalScale;

                for (int i = 0; i < OnDraw.Count; i++)
                    OnDraw[i](ev, transform);
            }
        }

        internal static void InitializeCache()
        {
            objectsArray = new List<Type>();
            // Id 0 is not used
            objectsArray.Add(null);

            objectsSynchronizedProperties = new Dictionary<Type, FieldInfo[]>();

            objectsDictionary = new Dictionary<Type, ushort>();
            nextObjectId = 1;

            synchronizedIds = new Dictionary<FieldInfo, ushort>();
        }

        internal static void CreateCache(Type type)
        {
            UInt16 nextId = 1;
            List<FieldInfo> fields = new List<FieldInfo>();
            fields.Add(null);
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(o => typeof(ISynchronized).IsAssignableFrom(o.FieldType)).OrderBy(o => o.Name))
            {
                fields.Add(field);
                synchronizedIds.Add(field, nextId);
                
                nextId++;
            }

            objectsSynchronizedProperties.Add(type, fields.ToArray());

            objectsArray.Add(type);
            objectsDictionary.Add(type, nextObjectId);

            nextObjectId++;
        }

        internal static FieldInfo GetSynchronizedField(Type objectType, ushort synchronizationId)
        {
            return GetSynchronizedFields(objectType)[synchronizationId];
        }

        internal static FieldInfo[] GetSynchronizedFields(Type objectType)
        {
            return objectsSynchronizedProperties[objectType];
        }

        internal static string GetNetworkDescriber()
        {
            StringBuilder builder = new StringBuilder("- OBJECTS\n");

            for (int i = 1; i < objectsArray.Count; i++)
            {
                var type = objectsArray[i];
                builder.Append(type.FullName);
                builder.Append('\n');
            }

            return builder.ToString();
        }
    }
}
