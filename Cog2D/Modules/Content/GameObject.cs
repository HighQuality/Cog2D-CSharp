using Cog.Modules.EventHost;
using Cog.Modules.Networking;
using Cog.Modules.Renderer;
using Cog.Modules.Resources;
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
    public abstract class GameObject : IBoundingBox
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
                        // Register us for draw calls
                        DrawTreeEntry = Scene.DrawTree.Insert(this, BoundingBox);
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

                    DrawTreeEntry.Owner.Remove(DrawTreeEntry);
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

        internal bool WasMoved;
        private Vector2 _localCoord;
        public Vector2 LocalCoord
        {
            get { return _localCoord; }
            set
            {
                if (Parent == null && !WasMoved && _localCoord != value)
                {
                    // Queue this item to be replaced within the draw QuadTree
                    Scene.MovedObjects.Push(this);
                    WasMoved = true;
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

        public QuadTreeEntry<GameObject> DrawTreeEntry;

        public Rectangle BoundingBox { get { return new Rectangle(WorldCoord, Size); } }
        public Vector2 Size { get; set; }
        public Scene Scene { get; internal set; }
        public bool DoRemove { get; private set; }
        public CogClient Owner { get; internal set; }
        public long Id { get; internal set; }
        public bool IsGlobal { get { return Id > 0; } }
        public bool IsLocal { get { return Id < 0; } }
        private List<IEventListener> registeredEvents;

        internal Action<DrawEvent, DrawTransformation> OnDraw;

        public GameObject()
        {
            Resources = Engine.ResourceHost.GetResourceCollection(GetType());
            ObjectName = GetType().Name;
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

            return new CreateObjectMessage(id);
        }

        internal static GameObject CreateFromMessage(CreateObjectMessage message)
        {
            Type objectType = objectsArray[message.ObjectType];
            GameObject obj = (GameObject)FormatterServices.GetUninitializedObject(objectType);

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
