﻿using Cog.Modules.EventHost;
using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Scenes
{
    public class SceneManager
    {
        public Scene CurrentScene { get; private set; }
        public int StackSize { get { return sceneStack.Count; } }
        private Stack<Scene> sceneStack = new Stack<Scene>();

        public SceneManager()
        {
        }
        
        public void TriggerButton(ButtonDownEvent ev)
        {
            if (CurrentScene != null)
            {
                CurrentScene.Interface.Press(ev.Position, ev);
            }

            if (!ev.Intercept)
                if (!Engine.EventHost.GetEvent<ButtonDownEvent>(ev.Button).Trigger(ev))
                    Engine.EventHost.GetEvent<ButtonDownEvent>().Trigger(ev);
        }

        public T CreateLocal<T>()
            where T : Scene, new()
        {
            T scene = (T)FormatterServices.GetUninitializedObject(typeof(T));
            scene.IsRemotelyCreated = false;
            Engine.GenerateLocalId(scene);

            // Invoke constructor
            typeof(T).GetConstructor(new Type[0]).Invoke(scene, new object[0]);

            return scene;
        }

        public T CreateGlobal<T>()
            where T : Scene, new()
        {
            if (Engine.IsClient)
                throw new InvalidOperationException("Can not create a global scene while connected to a server!");

            T scene = (T)FormatterServices.GetUninitializedObject(typeof(T));
            scene.IsRemotelyCreated = false;
            Engine.GenerateGlobalId(scene);

            // Invoke constructor
            typeof(T).GetConstructor(new Type[0]).Invoke(scene, new object[0]);

            return scene;
        }

        public void Push(Scene scene)
        {
            sceneStack.Push(scene);
            CurrentScene = scene;
        }

        public Scene Pop()
        {
            var scene = sceneStack.Pop();

            if (sceneStack.Count > 0)
            {
                CurrentScene = sceneStack.Peek();
                if (CurrentScene.DoRemove)
                    Pop();
            }
            else
                CurrentScene = null;

            return scene;
        }

        public void PopAll()
        {
            sceneStack.Clear();
            CurrentScene = null;
        }
    }
}
