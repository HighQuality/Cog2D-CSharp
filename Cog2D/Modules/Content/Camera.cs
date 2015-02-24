using Cog.Editor;
using Cog.Modules.EventHost;
using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    [HideInEditor()]
    public class Camera : GameObject
    {
        public IEventListener beginDrawEvent;

        public Camera()
        {
        }

        internal void Bind()
        {
            beginDrawEvent = RegisterEvent<BeginDrawEvent>(int.MaxValue - 1, BeginDraw);
        }
        
        internal void Unbind()
        {
            beginDrawEvent.Cancel();
            beginDrawEvent = null;
        }

        private void BeginDraw(BeginDrawEvent ev)
        {
            Engine.Window.RenderTarget.SetTransformation(WorldCoord, WorldScale, WorldRotation);
        }
    }
}
