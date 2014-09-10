using Square.Modules.Content;
using Square.Modules.EventHost;
using Square.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Scenes
{
    public class Scene
    {
        public LinkedList<GameObject> Objects = new LinkedList<GameObject>();
        public EventModule EventModule = new EventModule();

        internal Scene()
        {
        }

        public void Update(UpdateEvent args)
        {
            var current = Objects.First;
            do
            {
                if (current.Value.DoRemove)
                    Objects.Remove(current);
                current = current.Next;
            }
            while (current != null);

            EventModule.GetEvent<UpdateEvent>().Trigger(new UpdateEvent(args));
        }

        public void Draw(DrawEvent args)
        {
            EventModule.GetEvent<DrawEvent>().Trigger(new DrawEvent(args));
        }

        public void AddObject(GameObject obj)
        {
            foreach (var component in obj.Components)
            {
                component.Value.RegisterFunctions(EventModule);
            }

            Objects.AddLast(obj);
        }

        public void Clear()
        {
            var current = Objects.First;
            do
            {
                if (current.Value.DoRemove)
                    current.Value.Remove();
            }
            while (current != null);

            Objects.Clear();
        }
    }
}
