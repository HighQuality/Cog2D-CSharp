using Square.Modules.EventHost;
using Square.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.SfmlRenderer
{
    public class SfmlRenderer : IRenderModule
    {
        public IWindow CreateWindow(string title, int width, int height, WindowStyle style, EventModule eventHost)
        {
            return new SfmlWindow(title, width, height, style, eventHost);
        }

        public ITexture LoadTexture(string filename)
        {
            return new SfmlTexture(filename);
        }

        public ITexture TextureFromImage(Image image)
        {
            return new SfmlTexture(image);
        }
    }
}
