﻿using Square.Scenes;
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
        public Dictionary<Type, ObjectComponent> Components = new Dictionary<Type, ObjectComponent>();
        public Scene Scene { get; private set; }
        public bool DoRemove { get; private set; }

        public GameObject(Scene scene)
        {
            this.Scene = scene;
            this.Scene.AddObject(this);
        }

        public T AddComponenet<T>()
            where T : ObjectComponent, new()
        {
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

        public void RemoveComponent<T>()
            where T : ObjectComponent
        {
            ObjectComponent comp;
            if (Components.TryGetValue(typeof(T), out comp))
            {
                comp.ComponentRemoved();
                comp.DeregisterFunctions();
                Components.Remove(typeof(T));
            }
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

            DoRemove = true;
        }
    }
}
