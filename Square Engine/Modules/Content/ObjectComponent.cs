using Square.Modules.EventHost;
using Square.Modules.Renderer;
using Square.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.Content
{
    public abstract class ObjectComponent
    {
        private List<IEventListener> registeredFunctions;
        private static Dictionary<Type, Action<EventModule, ObjectComponent>> registererCache;

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
        public virtual void Draw(IRenderTarget renderTarget) { }

        public virtual void ObjectRemoved() { }
        public virtual void ComponentRemoved() { }

        internal void DeregisterFunctions()
        {
            // Cancel registered event listeners
            for (int i = registeredFunctions.Count - 1; i >= 0; i--)
                registeredFunctions[i].Cancel();
            registeredFunctions.Clear();
        }

        internal void RegisterFunctions(EventModule events)
        {
            Action<EventModule, ObjectComponent> registerer = null;
            bool didRegister = false;
            if (registererCache != null)
            {
                // Try to get a cached event registerer
                if (registererCache.TryGetValue(GetType(), out registerer))
                {
                    // Components without events are still cached
                    if (registerer != null)
                    {
                        registerer(events, this);
                    }
                    didRegister = true;
                }
            }
            else
                registererCache = new Dictionary<Type, Action<EventModule, ObjectComponent>>();

            if (!didRegister)
            {
                HashSet<string> alreadyRegistered = new HashSet<string>();
                
                // Iterate all of this component's methods to figure out which ones to register as events
                foreach (var method in GetType().GetMethods())
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

                    // All tests were passed, register the method

                    string func = method.Name;
                    if (func == "Update")
                        registerer += (ev, comp) => comp.RegisterEvent<UpdateEvent>(0, e => comp.Update(e.DeltaTime));
                    else if (func == "Draw")
                        registerer += (ev, comp) => comp.RegisterEvent<DrawEvent>(0, e => comp.Draw(e.RenderTarget));
                    else if (func == "KeyDown")
                        registerer += (ev, comp) => comp.RegisterEvent<KeyDownEvent>(0, e => { if (comp.KeyDown(e.Key)) { e.KeyUpEvent = () => comp.KeyUp(e.Key); e.Intercept = true; } });
                    else
                        Console.WriteLine("Tried to register function with no registration handler: " + func);
                }

                // Register our events
                if (registerer != null)
                {
                    registerer(events, this);
                }
                // Cache the registerer
                registererCache.Add(GetType(), registerer);
            }
        }

        public EventListener<T> RegisterEvent<T>(int priority, Action<T> action)
            where T : EventParameters
        {
            var listener = Scene.EventModule.RegisterEvent(priority, action);
            Scene.AddEventStrength<T>(listener);
            if (registeredFunctions == null)
                registeredFunctions = new List<IEventListener>();
            registeredFunctions.Add(listener);
            return listener;
        }
    }
}
