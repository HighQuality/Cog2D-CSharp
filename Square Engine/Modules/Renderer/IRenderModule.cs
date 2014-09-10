using Square.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.Modules.Renderer
{
    public interface IRenderModule
    {
        /// <summary>
        /// Creates a Window with the specified parameters
        /// </summary>
        /// <param name="title">The Title</param>
        /// <param name="width">The Inner Width in pixels</param>
        /// <param name="height">The Inner Height in pixels</param>
        /// <param name="style">The style of the window</param>
        /// <returns>Instantiated Window</returns>
        IWindow CreateWindow(string title, int width, int height, WindowStyle style, EventModule eventHost);

        /// <summary>
        /// Loads a texture from the specified filename.
        /// Cache has priority.
        /// </summary>
        /// <param name="filename">The filename of the texture to load</param>
        /// <returns>An interface to the texture</returns>
        ITexture LoadTexture(string filename);
    }
}
