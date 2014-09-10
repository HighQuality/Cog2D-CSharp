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
        private List<GameObject> objects = new List<GameObject>();
        public EventModule EventModule = new EventModule();

        internal Scene()
        {
        }

        public void Update(UpdateEvent args)
        {
            EventModule.GetEvent<UpdateEvent>().Trigger(new UpdateEvent(args));
        }

        public void Draw(DrawEvent args)
        {
            EventModule.GetEvent<DrawEvent>().Trigger(new DrawEvent(args));
        }
    }
}
