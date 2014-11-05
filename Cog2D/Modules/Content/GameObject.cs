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
                    }
                }
                else if (value != null)
                {
                    // If we didn't have a parent already and was assigned one, iterate through the base object LinkedList backwards and get rid of our entry
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
        
        public Vector2 LocalCoord { get; set; }
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

        public T AddComponent<T>()
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
                    children[i].Draw(ev, transform);
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
