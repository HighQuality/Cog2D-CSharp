using Cog.Modules.EventHost;
using Cog.Modules.Networking;
using Cog.Modules.Renderer;
using Cog.Scenes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public abstract class GameObject
    {
        private static Dictionary<Type, UInt16> objectsDictionary;
        private static List<Type> objectsArray;
        private static UInt16 nextObjectId;

        public string ObjectName;
        private GameObject _parent;
        public GameObject Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != null)
                    _parent.children.Remove(this);
                _parent = value;
                if (_parent != null)
                {
                    if (_parent.children == null)
                        _parent.children = new List<GameObject>();
                    _parent.children.Add(this);
                    // Our local coordinate isn't updated yet
                    LocalCoord = _parent.WorldCoord - WorldCoord;
                    LocalRotation = _parent.WorldRotation - WorldRotation;
                }
                else
                {
                    LocalCoord = WorldCoord;
                    LocalRotation = WorldRotation;
                }
            }
        }

        private List<GameObject> children;
        
        public Vector2 LocalCoord { get; set; }
        public virtual Vector2 WorldCoord
        {
            get { return Parent.WorldCoord + (LocalCoord * Parent.WorldScale).Rotate(Parent.WorldRotation); }
            set { LocalCoord = (value - Parent.WorldCoord).Rotate(-Parent.WorldRotation); }
        }

        public Angle LocalRotation { get; set; }
        public virtual Angle WorldRotation
        {
            get { if (Parent != null) return Parent.WorldRotation + LocalRotation; return LocalRotation; }
            set
            {
                if (Parent != null)
                    LocalRotation = value - Parent.WorldRotation;
                else
                    LocalRotation = value;
            }
        }

        private Vector2 _localScale = Vector2.One;
        public Vector2 LocalScale { get { return _localScale; } set { _localScale = value; } }
        public virtual Vector2 WorldScale
        {
            get { if (Parent != null) return Parent.WorldScale * LocalScale; return LocalScale; }
            set
            {
                if (Parent != null)
                    LocalScale = value / Parent.WorldScale;
                else
                    LocalScale = value;
            }
        }

        public Vector2 Size { get; set; }
        public Dictionary<Type, ObjectComponent> Components = new Dictionary<Type, ObjectComponent>();
        public Scene Scene { get; internal set; }
        public bool DoRemove { get; private set; }
        public CogClient Owner { get; internal set; }
        public long Id { get; internal set; }
        public bool IsGlobal { get { return Id > 0; } }
        public bool IsLocal { get { return Id < 0; } }
        public bool IsComponentsLocked { get; internal set; }
        private List<IEventListener> registeredEvents;

        internal Action<DrawEvent, DrawTransformation> OnDraw;

        public GameObject()
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

        public T AddComponenet<T>()
            where T : ObjectComponent, new()
        {
            if (IsComponentsLocked)
                throw new Exception("You may not add a component to this object at this time!");
            if (Components.ContainsKey(typeof(T)))
                throw new InvalidOperationException("The Game Object \"" + ObjectName + "\" already contains a \"" + typeof(T).FullName + "\" component");
            // Create a new instance of T without invoking the constructor
            T component = (T)FormatterServices.GetUninitializedObject(typeof(T));
            // Set the object that this component is meant to interact with
            component.GameObject = this;
            // Invokes the default construtor; new()-constraint ensures a constructor with 0 parameters exist
            typeof(T).GetConstructor(new Type[0]).Invoke((object)component, new object[0]);
            Components.Add(typeof(T), component);

            // Register the component's events
            component.RegisterFunctions(Scene.EventModule);

            return component;
        }
                
        public void Remove()
        {
            foreach (var component in Components)
            {
                component.Value.ComponentRemoved();
                component.Value.ObjectRemoved();
                component.Value.DeregisterFunctions();
            }
            Components.Clear();

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
        
        internal CreateObjectMessage CreateCreationMessage()
        {
            var id = objectsDictionary[GetType()];

            UInt16[] componentIds = new ushort[Components.Count];
            byte[][] componentDatas = new byte[Components.Count][];

            int componentNumber = 0;
            foreach (var component in Components.Values)
            {
                var type = component.GetType();
                var serializer = ObjectComponent.GetSerializer(type);
                componentIds[componentNumber] = serializer.Id;
                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        serializer.Serialize(component, writer);
                    }
                    componentDatas[componentNumber] = stream.ToArray();
                }

                componentNumber++;
            }

            return new CreateObjectMessage(id, componentIds, componentDatas);
        }

        internal static GameObject CreateFromMessage(CreateObjectMessage message)
        {
            Type objectType = objectsArray[message.ObjectType];
            GameObject obj = (GameObject)FormatterServices.GetUninitializedObject(objectType);

            for (UInt16 i = 0; i < message.Components.Length; i++)
            {
                ObjectComponent comp = (ObjectComponent)FormatterServices.GetUninitializedObject(ObjectComponent.GetSerializer(message.Components[i]).Type);

                using (MemoryStream stream = new MemoryStream(message.ComponentDatas[i]))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        ObjectComponent.GetSerializer(i).Deserialize(comp, reader);
                    }
                }
            }

            return obj;
        }
        
        internal void Draw(DrawEvent ev, DrawTransformation transform)
        {
            if (children != null)
            {
                transform.WorldCoord += (LocalCoord * transform.ParentWorldScale).Rotate(transform.ParentWorldRotation);
                transform.WorldRotation += LocalRotation;
                transform.WorldScale *= LocalScale;

                if (OnDraw != null)
                    OnDraw(ev, transform);

                transform.ParentWorldCoord = transform.WorldCoord;
                transform.ParentWorldRotation = transform.WorldRotation;
                transform.ParentWorldScale = transform.WorldScale;

                int c = children.Count;
                for (int i = 0; i < c; i++)
                {
                    children[i].Draw(ev, transform);
                }
            }
            else if (OnDraw != null)
            {
                transform.WorldCoord += (LocalCoord * transform.ParentWorldScale).Rotate(transform.ParentWorldRotation);
                transform.WorldRotation += LocalRotation;
                transform.WorldScale *= LocalScale;

                OnDraw(ev, transform);
            }
        }

        internal static void InitializeCache()
        {
            objectsArray = new List<Type>();
            objectsDictionary = new Dictionary<Type, ushort>();
            nextObjectId = 1;
        }

        internal static void CreateCache(Type type)
        {
            objectsArray.Add(type);
            objectsDictionary.Add(type, nextObjectId++);
        }
    }
}
