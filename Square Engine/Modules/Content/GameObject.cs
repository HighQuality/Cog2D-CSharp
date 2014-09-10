using Square.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.Content
{
    public class GameObject
    {
        public string ObjectName;
        public Vector2 WorldCoord { get; set; }
        public Dictionary<Type, GameComponent> Components = new Dictionary<Type, GameComponent>();
        public Scene Scene { get; private set; }
        public bool DoRemove { get; private set; }

        public GameObject(Scene scene)
        {
            this.Scene = scene;
            this.Scene.AddObject(this);
        }

        public T AddComponenet<T>()
            where T : GameComponent, new()
        {
            if (Components.ContainsKey(typeof(T)))
                throw new InvalidOperationException("The Game Object \"" + ObjectName + "\" already contains a \"" + typeof(T).FullName + "\" component");
            T component = (T)FormatterServices.GetUninitializedObject(typeof(T));
            component.GameObject = this;
            // Invokes the default construtor; new()-constraint ensures a constructor with 0 parameters exist
            typeof(T).GetConstructor(new Type[0]).Invoke((object)component, new object[0]);
            Components.Add(typeof(T), component);

            component.RegisterFunctions(Scene.EventModule);

            return component;
        }

        public void RemoveComponent<T>()
            where T : GameComponent
        {
            GameComponent comp;
            if (Components.TryGetValue(typeof(T), out comp))
            {
                comp.ObjectRemoved();
                Components.Remove(typeof(T));
            }
        }
        
        public void Remove()
        {
            foreach (var component in Components)
                component.Value.ObjectRemoved();
            Components.Clear();

            DoRemove = true;
        }
    }
}
