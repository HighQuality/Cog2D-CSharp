using Square.Modules.EventHost;
using Square.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.Content
{
    public abstract class GameComponent
    {
        private Dictionary<string, IEventListener> registeredFunctions = new Dictionary<string, IEventListener>();
        
        public GameObject GameObject { get; internal set; }
        public Vector2 WorldCoord { get { return GameObject.WorldCoord; } set { GameObject.WorldCoord = value; } }

        public GameComponent()
        {
        }

        [ComponentEvent(true)]
        public virtual void KeyDown(Keyboard.Key key) { }
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
                listener = GameObject.Scene.EventModule.RegisterEvent<KeyDownEvent>(0, e => { KeyDown(e.Key); e.KeyUpEvent = () => KeyUp(e.Key); e.Intercept = true; });
            else
                Console.WriteLine("Tried to register function with no registration handler: " + functionName);

            if (listener != null)
                registeredFunctions.Add(functionName, listener);
        }

        internal void RegisterFunctions(EventModule events)
        {
            var currentType = GetType();

            do
            {
                foreach (var method in currentType.GetMethods())
                {
                    if (method.DeclaringType != currentType)
                        continue;
                    var eventAttribute = (ComponentEventAttribute)method.GetCustomAttributes(typeof(ComponentEventAttribute), true).FirstOrDefault();
                    if (eventAttribute == null)
                        continue;
                    if (eventAttribute.RequireOverride)
                    {
                        var baseDefinition = method.GetBaseDefinition();
                        if (baseDefinition == null || baseDefinition.DeclaringType != typeof(GameComponent))
                            continue;
                    }
                    RegisterFunction(method.Name);
                }
                currentType = currentType.BaseType;
            }
            while (currentType != typeof(GameComponent));
        }
    }
}
