using Square.Modules.EventHost;
using Square.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Scenes
{
    public class SceneManager
    {
        public Scene CurrentScene { get; private set; }
        private Stack<Scene> sceneStack = new Stack<Scene>();

        public SceneManager()
        {
            Push(new Scene());

            Engine.EventHost.GetEvent<UpdateEvent>().Register(0, RerouteEvent);
            Engine.EventHost.GetEvent<DrawEvent>().Register(0, RerouteEvent);
            Engine.EventHost.GetEvent<KeyDownEvent>().Register(0, RerouteEvent);
            Engine.EventHost.GetEvent<CloseButtonEvent>().Register(0, RerouteEvent);
            Engine.EventHost.GetEvent<ExitEvent>().Register(0, RerouteEvent);
        }

        private void RerouteEvent<T>(T args)
            where T : EventParameters
        {
            if (CurrentScene != null)
            {
                CurrentScene.EventModule.GetEvent<T>().Trigger(args);
            }
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
