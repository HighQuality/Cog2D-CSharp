using Square.Modules.EventHost;
using Square.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.Content
{
    public abstract class ObjectComponent
    {
        private Dictionary<string, IEventListener> registeredFunctions = new Dictionary<string, IEventListener>();
        
        public GameObject GameObject { get; internal set; }
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

        [ComponentEvent(false)]
        public virtual void ObjectRemoved()
        {
            // Cancel registered event listeners
            foreach (var listener in registeredFunctions.Values)
                listener.Cancel();
            registeredFunctions.Clear();
        }

        private void RegisterFunction(string functionName)
        {
            // Don't do anything if the function is already registered
            if (registeredFunctions.ContainsKey(functionName))
                return;

            IEventListener listener = null;
            if (functionName == "Update")
                listener = GameObject.Scene.EventModule.RegisterEvent<UpdateEvent>(0, e => Update(e.DeltaTime));
            else if (functionName == "Draw")
                listener = GameObject.Scene.EventModule.RegisterEvent<DrawEvent>(0, e => Draw(e.RenderTarget));
            else if (functionName == "KeyDown")
                listener = GameObject.Scene.EventModule.RegisterEvent<KeyDownEvent>(0, e => { if (KeyDown(e.Key)) { e.KeyUpEvent = () => KeyUp(e.Key); e.Intercept = true; } });
            else
                Console.WriteLine("Tried to register function with no registration handler: " + functionName);

            if (listener != null)
                registeredFunctions.Add(functionName, listener);
        }

        internal void RegisterFunctions(EventModule events)
        {
            foreach (var method in GetType().GetMethods())
            {
                // Only register each method once (Derived types can override it multiple times)
                if (registeredFunctions.ContainsKey(method.Name))
                    continue;
                //Require a ComponentEventAttribute to register the method
                var eventAttribute = (ComponentEventAttribute)method.GetCustomAttributes(typeof(ComponentEventAttribute), true).FirstOrDefault();
                if (eventAttribute == null)
                    continue;
                // If it requires an overload to register, enforce it
                if (eventAttribute.RequireOverride)
                {
                    if (method.GetBaseDefinition() == null)
                        continue;
                }
                // All tests were passed, register the method
                RegisterFunction(method.Name);
                Console.WriteLine(method.Name + " in " + GetType());
            }
        }
    }
}
