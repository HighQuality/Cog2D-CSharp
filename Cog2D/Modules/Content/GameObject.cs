using Cog.Modules.EventHost;
using Cog.Modules.Networking;
using Cog.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public abstract class GameObject
    {
        public string ObjectName;
        public GameObject Parent;
        public Vector2 LocalCoord { get; set; }
        public Vector2 WorldCoord { get { if (Parent != null) return Parent.WorldCoord + LocalCoord; return LocalCoord; } set { if (Parent != null) LocalCoord = value - Parent.WorldCoord; LocalCoord = value; } }
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
    }
}
