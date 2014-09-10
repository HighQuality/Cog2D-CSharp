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

        public virtual void KeyDown(Keyboard.Key key) { }
        public virtual void KeyUp(Keyboard.Key key) { }
        public virtual void Update(float deltaTime) { }
        public virtual void Draw(IRenderTarget renderTarget) { }
        
        public virtual void ObjectRemoved()
        {
            // Cancel registered event listeners
            foreach (var listener in registeredFunctions.Values)
                listener.Cancel();
            registeredFunctions.Clear();
        }

        internal void RegisterFunction(string functionName)
        {
            // Don't do anything if the function is already registered
            if (registeredFunctions.ContainsKey(functionName))
                return;

            if (functionName == "Update")
                registeredFunctions.Add(functionName, GameObject.Scene.EventModule.RegisterEvent<UpdateEvent>(0, e => Update(e.DeltaTime)));
            else if (functionName == "Draw")
                registeredFunctions.Add(functionName, GameObject.Scene.EventModule.RegisterEvent<DrawEvent>(0, e => Draw(e.RenderTarget)));
            else if (functionName == "KeyDown")
                registeredFunctions.Add(functionName, GameObject.Scene.EventModule.RegisterEvent<KeyDownEvent>(0, e => { KeyDown(e.Key); e.KeyUpEvent = () => KeyUp(e.Key); e.Intercept = true; }));
            else
                Console.WriteLine("Tried to register function with no registration handler: " + functionName);
        }
    }
}
