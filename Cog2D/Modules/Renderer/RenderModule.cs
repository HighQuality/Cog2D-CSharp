using Cog.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Renderer
{
    public abstract class RenderModule
    {
        /// <summary>
        /// Creates a Window with the specified parameters
        /// </summary>
        /// <param name="title">The Title</param>
        /// <param name="width">The Inner Width in pixels</param>
        /// <param name="height">The Inner Height in pixels</param>
        /// <param name="style">The style of the window</param>
        /// <returns>Instantiated Window</returns>
        public abstract Window CreateWindow(string title, int width, int height, WindowStyle style, EventModule eventHost);

        /// <summary>
        /// Loads a texture from the specified filename.
        /// </summary>
        /// <param name="filename">The filename of the texture to load</param>
        /// <returns>An interface to the texture</returns>
        public abstract Texture LoadTexture(string filename);

        /// <summary>
        /// Loads a texture from binary data.
        /// </summary>
        public abstract Texture LoadTexture(byte[] data);

        /// <summary>
        /// Generates a texture from the specified image.
        /// </summary>
        /// <param name="image">The image to generate a texture from</param>
        /// <returns>A generated texture</returns>
        public abstract Texture TextureFromImage(Image image);
    }
}
