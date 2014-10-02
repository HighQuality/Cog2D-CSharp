using Square.Modules.EventHost;
using Square.Modules.Renderer;
using Square.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.Content
{
    public abstract class ObjectComponent
    {
        private List<IEventListener> registeredFunctions;
        internal static Dictionary<Type, Action<EventModule, ObjectComponent>> RegistratorCache;

        public GameObject GameObject { get; internal set; }
        public Scene Scene { get { return GameObject.Scene; } }
        public Vector2 WorldCoord { get { return GameObject.WorldCoord; } set { GameObject.WorldCoord = value; } }

        public ObjectComponent()
        {
        }

        [ComponentEvent(true)]
        public virtual bool KeyDown(Keyboard.Key key) { return false; }
        public virtual void KeyUp(Keyboard.Key key) { }
        [ComponentEvent(true)]
        public virtual void Update(float deltaTime) { }
        [ComponentEvent(true)]
        public virtual void PhysicsUpdate(float deltaTime) { }
        [ComponentEvent(true)]
        public virtual void Draw(IRenderTarget renderTarget) { }
        [ComponentEvent(true)]
        public virtual void DrawInterface(IRenderTarget renderTarget) { }

        public virtual void ObjectRemoved() { }
        public virtual void ComponentRemoved() { }

        internal void DeregisterFunctions()
        {
            // Cancel registered event listeners
            for (int i = registeredFunctions.Count - 1; i >= 0; i--)
                registeredFunctions[i].Cancel();
            registeredFunctions.Clear();
        }

        internal static Action<EventModule, ObjectComponent> CreateRegistrator(Type type)
        {
            Action<EventModule, ObjectComponent> registrator = null;
            HashSet<string> alreadyRegistered = new HashSet<string>();

            // Iterate all of this component's methods to figure out which ones to register as events
            foreach (var method in type.GetMethods())
            {
                //Require a ComponentEventAttribute to register the method
                var eventAttribute = (ComponentEventAttribute)method.GetCustomAttributes(typeof(ComponentEventAttribute), true).FirstOrDefault();
                if (eventAttribute == null)
                    continue;
                // If it requires an overload to register, enforce it
                if (eventAttribute.RequireOverride)
                {
                    var baseDef = method.GetBaseDefinition();
                    if (baseDef == null || baseDef == method || baseDef.DeclaringType == method.DeclaringType)
                        continue;
                }

                // Check if it's already registered (Multiple overrides) / create a dictionary for registered functions
                if (alreadyRegistered.Contains(method.Name))
                    continue;
                alreadyRegistered.Add(method.Name);

                // All tests were passed, add the method regitrator
                string func = method.Name;
                if (func == "Update")
                    registrator += (ev, comp) => comp.RegisterEvent<UpdateEvent>(0, e => comp.Update(e.DeltaTime));
                else if (func == "PhysicsUpdate")
                    registrator += (ev, comp) => comp.RegisterEvent<PhysicsUpdateEvent>(0, e => comp.PhysicsUpdate(e.DeltaTime));
                else if (func == "Draw")
                    registrator += (ev, comp) => comp.RegisterEvent<DrawEvent>(0, e => comp.Draw(e.RenderTarget));
                else if (func == "KeyDown")
                    registrator += (ev, comp) => comp.RegisterEvent<KeyDownEvent>(0, e => { if (comp.KeyDown(e.Key)) { e.KeyUpEvent = () => comp.KeyUp(e.Key); e.Intercept = true; } });
                else if (func == "DrawInterface")
                    registrator += (ev, comp) => comp.RegisterEvent<DrawInterfaceEvent>(0, e => comp.DrawInterface(e.RenderTarget));
                else
                    Console.WriteLine("Tried to register function with no registration handler: " + func);
            }

            // Cache the registerer
            RegistratorCache.Add(type, registrator);

            return registrator;
        }
        
        internal void RegisterFunctions(EventModule events)
        {
            Action<EventModule, ObjectComponent> registrator = null;
            bool didRegister = false;
            if (RegistratorCache != null)
            {
                // Try to get a cached event registerer
                if (RegistratorCache.TryGetValue(GetType(), out registrator))
                {
                    // Components without events are still cached
                    if (registrator != null)
                    {
                        registrator(events, this);
                    }
                    didRegister = true;
                }
            }

            if (!didRegister)
            {
                registrator = CreateRegistrator(GetType());

                // Register our events
                if (registrator != null)
                {
                    registrator(events, this);
                }
            }
        }
        
        public EventListener<T> RegisterEvent<T>(int priority, Action<T> action)
            where T : EventParameters
        {
            return RegisterEvent<T>(EventModule.DefaultUniqueIdentifier, priority, action);
        }

        public EventListener<T> RegisterEvent<T>(object uniqueIdentifier, int priority, Action<T> action)
            where T : EventParameters
        {
            var listener = Scene.EventModule.RegisterEvent(uniqueIdentifier, priority, action);
            Scene.AddEventStrength<T>(listener);
            if (registeredFunctions == null)
                registeredFunctions = new List<IEventListener>();
            registeredFunctions.Add(listener);
            return listener;
        }

        public KeyCapture CaptureKey(Keyboard.Key key, int priority, CaptureRelayMode relayMode)
        {
            if (relayMode != CaptureRelayMode.NoRelay && !Engine.IsNetworkGame)
                throw new ArgumentOutOfRangeException("Only CaptureRelayMode.NoRelay may be used in non-multiplayer games!");
            return new KeyCapture(GameObject, key, priority, relayMode);
        }
    }
}
