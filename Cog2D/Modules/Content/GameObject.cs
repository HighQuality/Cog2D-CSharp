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
    public abstract class GameObject<SceneType> : GameObject
        where SceneType : Scene
    {
        new public SceneType Scene { get { return (SceneType)base.Scene; } }
    }

    public abstract class GameObject : IBoundingBox, IIdentifier
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
                        if (!IsScheduledForDrawCellMove)
                        {
                            IsScheduledForDrawCellMove = true;
                            Scene.DrawCellMoveQueue.Enqueue(new DrawCellMoveInfo
                            {
                                Object = this,
                                IsInitialPlacement = true
                            });
                        }
                    }
                }
                else if (value != null)
                {
                    // If we got a parent now get rid of our draw entries

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

        public bool IsVisible = true;
        internal bool IsScheduledForDrawCellMove;
        internal DrawCell CurrentDrawCell;

        private Vector2 _localCoord;
        public Vector2 LocalCoord
        {
            get { return _localCoord; }
            set
            {
                if (Parent == null)
                {
                    if (!IsScheduledForDrawCellMove)
                    {
                        // If we moved to another draw cell schedule us to be moved
                        if ((int)Math.Floor(value.X / (float)DrawCell.DrawCellSize) != CurrentDrawCell.X ||
                            (int)Math.Floor(value.Y / (float)DrawCell.DrawCellSize) != CurrentDrawCell.Y)
                        {
                            IsScheduledForDrawCellMove = true;
                            Scene.DrawCellMoveQueue.Enqueue(new DrawCellMoveInfo
                            {
                                Object = this,
                                IsInitialPlacement = false
                            });
                        }
                    }
                }
                _localCoord = value;
            }
        }
        public Vector2 WorldCoord
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
        public Angle WorldRotation
        {
            get
            {
                if (Parent == null)
                    return WorldScale.X < 0f ? -LocalRotation : LocalRotation;
                return WorldScale.X < 0f ? -(Parent.WorldRotation + LocalRotation) : (Parent.WorldRotation + LocalRotation);
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
        public Vector2 WorldScale
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

        public Rectangle BoundingBox { get { return new Rectangle(WorldCoord - Size / 2f, Size); } }
        public Vector2 Size { get; set; }
        public Scene Scene { get; internal set; }
        public bool DoRemove { get; private set; }
        public event Action OnRemoved;
        public event Action OnRemovalComplete;
        /// <summary>
        /// Gets the ID of this object.
        /// SET IS ONLY FOR INTERNAL ENGINE USE
        /// </summary>
        public long Id { get; set; }
        public bool IsGlobal { get { return Id > 0; } }
        public bool IsLocal { get { return Id < 0; } }
        private List<IEventListener> registeredEvents;

        private float _depth = 1f;
        public float Depth
        {
            get { return _depth; }
            set
            {
                _depth = value;
            }
        }

        internal List<Action<DrawEvent, DrawTransformation>> OnDraw;

        public GameObject()
        {
            if (Id == 0)
                throw new InvalidOperationException("GameObjet.Id was never set! Did you create the object through one of the Scene creation methods?");
            if (InitializationData == null)
                throw new InvalidOperationException("GameObject.InitalizationData was never set! Did you create the object through one of the Scene creation methods?");

            var type = GetType();
            
            Resources = Engine.ResourceHost.GetResourceCollection(type);

            // ObjectName can be set through deserialization before the object was created, assign a default name if nothing was set
            if (ObjectName == null)
                ObjectName = type.Name;

            var fields = objectsSynchronizedProperties[type];

            for (int i = 1; i < fields.Length; i++)
                fields[i].SetValue(this, InitializationData.SynchronizedFields[i - 1]);

            if (InitializationData.UserData != null)
                using (MemoryStream userDataStream = new MemoryStream(InitializationData.UserData))
                using (BinaryReader userDataReader = new BinaryReader(userDataStream))
                    ReadUserData(userDataReader);

            // Initialization has finished, get rid of data that is no longer necessary
            InitializationData = null;
        }

        public virtual void Initialize()
        {
        }

        public virtual void CreationData(object[] creationData)
        {
        }

        public void AddDraw(Action<DrawEvent, DrawTransformation> action)
        {
            if (OnDraw == null)
                OnDraw = new List<Action<DrawEvent, DrawTransformation>>();
            OnDraw.Add(action);
        }

        public void Remove()
        {
            if (Engine.IsClient && IsGlobal)
                throw new InvalidOperationException("Can not Remove a global object when connected to a server!");
            if (DoRemove)
                return;

            ForceRemove();

            if (Engine.IsServer && IsGlobal)
            {
                Send(new RemoveObjectMessage(this));
                SubscribedClients.Clear();
            }
        }

        internal void ForceRemove()
        {
            if (!DoRemove)
            {
                // Remove children
                if (children != null)
                    for (int i = children.Count - 1; i >= 0; i--)
                        children[i].ForceRemove();

                // Events are safe to cancel mid-enumeration, just sets flag
                if (registeredEvents != null)
                {
                    for (int i = registeredEvents.Count - 1; i >= 0; i--)
                        registeredEvents[i].Cancel();
                    registeredEvents = null;
                }

                // Schedule us for removal during the next frame
                // Ensures no data structures are modified mid-enumeration
                Engine.InvokeTimed(0f, o =>
                {
                    if (Parent != null)
                        Parent.children.Remove(this);

                    if (OnRemovalComplete != null)
                        OnRemovalComplete();
                });

                // Engine's object dictionary is not for enumeration and is therefore safe to remove immediately
                Engine.FreeId(Id);

                if (Parent == null)
                {
                    // Make sure we're scheduled for a draw cell move, removed objects are removed from their draw cell
                    if (!IsScheduledForDrawCellMove)
                    {
                        Scene.DrawCellMoveQueue.Enqueue(new DrawCellMoveInfo
                        {
                            IsInitialPlacement = false,
                            Object = this
                        });
                    }
                }

                DoRemove = true;

                if (OnRemoved != null)
                    OnRemoved();
            }
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
            foreach (var client in SubscribedClients)
                client.Send(message);
        }

        public void Send<T>(T message, CogClient except)
            where T : NetworkMessage
        {
            foreach (var client in SubscribedClients)
                if (client != except)
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
            if (!IsGlobal || !Scene.IsGlobal)
                throw new InvalidOperationException("Tried to create an object creation message from a local object or from an object in a local scene!");

            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    Serialize(writer);

                    var msg = new CreateObjectMessage(stream.ToArray());
                    msg.Scene = Scene;
                    msg.TypeId = IdFromType(GetType());
                    msg.ObjectId = Id;

                    return msg;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1}, ID {2})", ObjectName, GetType().Name, Id);
        }

        internal void Serialize(BinaryWriter writer)
        {
            writer.Write(ObjectName);

            if (Parent != null)
                writer.Write((long)Parent.Id);
            else
                writer.Write((long)0);
            
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
                ISynchronized sync = (ISynchronized)fields[i].GetValue(this);
                sync.Serialize(writer);
            }

            using (MemoryStream userDataStream = new MemoryStream())
            {
                using (BinaryWriter userDataWriter = new BinaryWriter(userDataStream))
                {
                    WriteUserData(userDataWriter);

                    var userData = userDataStream.ToArray();
                    writer.Write((UInt32)userData.Length);
                    writer.Write((byte[])userData);
                }
            }
        }

        internal void Deserialize(BinaryReader reader)
        {
            ObjectName = reader.ReadString();

            long parentId = reader.ReadInt64();
            if (parentId != 0)
            {
                // TODO: Change this to InitialSetParent?
                Parent = Engine.Resolve<GameObject>(parentId);
                if (Parent == null)
                    throw new Exception("Could not find parent with ID " + parentId.ToString());
            }

            // Transformation
            Vector2 coord,
                scale;
            float rotDegree;

            coord.X = reader.ReadSingle();
            coord.Y = reader.ReadSingle();
            rotDegree = reader.ReadSingle();
            scale.X = reader.ReadSingle();
            scale.Y = reader.ReadSingle();

            LocalCoord = coord;
            LocalRotation = Angle.FromDegree(rotDegree);
            LocalScale = scale;

            // Read properties
            var fields = objectsSynchronizedProperties[GetType()];
            for (int i = 1; i < fields.Length; i++)
            {
                InitializationData.SynchronizedFields[i - 1].Deserialize(reader);
            }

            var userSize = reader.ReadUInt32();
            if (userSize > 0)
                InitializationData.UserData = reader.ReadBytes((int)userSize);
        }

        public virtual void ReadUserData(BinaryReader reader)
        {
        }

        public virtual void WriteUserData(BinaryWriter writer)
        {
        }

        public IEnumerable<T> EnumerateChildren<T>()
            where T : GameObject
        {
            for (int i = 0; i < children.Count; i++)
                if (children[i] is T)
                    yield return (T)children[i];
        }

        internal static Type TypeFromId(ushort id)
        {
            return objectsArray[(int)id];
        }

        internal static ushort IdFromType(Type type)
        {
            return objectsDictionary[type];
        }
        
        internal void Draw(DrawEvent ev, DrawTransformation transform, List<DrawOperation> drawList)
        {
            if (!IsVisible)
                return;

            if (children != null)
            {
                transform.WorldCoord += (LocalCoord * transform.ParentWorldScale).Rotate(transform.ParentWorldRotation);
                transform.WorldScale *= LocalScale;
                transform.WorldRotation += transform.WorldScale.X < 0f ? -LocalRotation : LocalRotation;

                if (OnDraw != null)
                {
                    // Copy the transformation variable, otherwise it'll be modified before OnDraw is called
                    var myTransform = transform;
                    drawList.Add(new DrawOperation
                    {
                        Depth = this.Depth,
                        ObjectId = Id,
                        Action = () =>
                        {
                            for (int i = 0; i < OnDraw.Count; i++)
                                OnDraw[i](ev, myTransform);
                        }
                    });
                }

                transform.ParentWorldCoord = transform.WorldCoord;
                transform.ParentWorldRotation = transform.WorldRotation;
                transform.ParentWorldScale = transform.WorldScale;

                int c = children.Count;
                for (int i = 0; i < c; i++)
                    children[i].Draw(ev, transform, drawList);
            }
            else if (OnDraw != null)
            {
                transform.WorldCoord += (LocalCoord * transform.ParentWorldScale).Rotate(transform.ParentWorldRotation);
                transform.WorldScale *= LocalScale;
                transform.WorldRotation += transform.WorldScale.X < 0f ? -LocalRotation : LocalRotation;

                var myTransform = transform;
                drawList.Add(new DrawOperation
                {
                    Depth = this.Depth,
                    ObjectId = this.Id,
                    Action = () =>
                    {
                        for (int i = 0; i < OnDraw.Count; i++)
                            OnDraw[i](ev, myTransform);
                    }
                });
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
