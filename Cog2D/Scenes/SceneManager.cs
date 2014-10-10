﻿using Cog.Modules.EventHost;
using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Scenes
{
    public class SceneManager
    {
        public Scene CurrentScene { get; private set; }
        private Stack<Scene> sceneStack = new Stack<Scene>();

        public SceneManager()
        {
            // Register events for rerouting
            Register<UpdateEvent>();
            Register<DrawEvent>();
            Register<DrawInterfaceEvent>();
            Register<KeyDownEvent>();
            Register<CloseButtonEvent>();
            Register<ExitEvent>();
        }

        private void Register<T>()
            where T : EventParameters
        {
            Engine.EventHost.GetEvent<T>().Register(0, RerouteEvent);
        }

        private void RerouteEvent<T>(T args)
            where T : EventParameters
        {
            if (CurrentScene != null)
            {
                CurrentScene.EventModule.GetEvent<T>().Trigger(args);
            }
        }

        public void TriggerButton(ButtonDownEvent ev)
        {
            if (CurrentScene != null)
            {
                CurrentScene.Interface.TriggerPress(ev.Position, ev);
            }

            if (!ev.Intercept)
                if (!Engine.EventHost.GetEvent<ButtonDownEvent>(ev.Button).Trigger(ev))
                    Engine.EventHost.GetEvent<ButtonDownEvent>().Trigger(ev);
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
                CurrentScene = sceneStack.Peek();
            else
                CurrentScene = null;
            return scene;
        }
    }
}
