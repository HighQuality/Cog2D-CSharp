using Cog.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Renderer
{
    public class DefaultRenderer : IRenderModule
    {
        public IWindow CreateWindow(string title, int width, int height, WindowStyle style, EventModule eventHost)
        {
            return null;
        }

        public ITexture LoadTexture(string filename)
        {
            return null;
        }

        public ITexture TextureFromImage(Image image)
        {
            return null;
        }
    }
}
