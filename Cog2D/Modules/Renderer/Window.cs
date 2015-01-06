using Cog.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Renderer
{
    public abstract class Window
    {
        public Window()
        {
            Engine.Renderer.AlphaBlend.ForceSet(RenderTarget);
        }

        /// <summary>
        /// Gets or Sets the title of the window
        /// </summary>
        public abstract string Title { get; set; }
        /// <summary>
        /// Gets or Sets the size of the window
        /// </summary>
        public abstract Vector2 Resolution { get; set; }
        /// <summary>
        /// Gets or Sets the position of the window
        /// </summary>
        public abstract Vector2 Position { get; set; }
        public abstract Vector2 MousePosition { get; set; }
        /// <summary>
        /// Gets or Sets the visibility of the window
        /// </summary>
        public abstract bool Visible { get; set; }
        /// <summary>
        /// Gets if the window is still open
        /// </summary>
        public abstract bool IsOpen { get; }
        public abstract bool VerticalSynchronization { get; set; }

        /// <summary>
        /// Gets the current render target
        /// </summary>
        public abstract IRenderTarget RenderTarget { get; }

        /// <summary>
        /// Commands the window to dispatch it's queued events
        /// </summary>
        public abstract void DispatchEvents();
        /// <summary>
        /// Commands the window to clear the back buffer to the specified color
        /// </summary>
        /// <param name="color">The color to clear to</param>
        public abstract void Clear(Color color);
        /// <summary>
        /// Commands the window to switch the backbuffer displaying rendered content
        /// </summary>
        public abstract void Display();
        /// <summary>
        /// Closes the window
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Commands the window to apply changes.
        /// Depending on the renderer, changes may be applied before this method is called.
        /// </summary>
        public abstract void ApplyChanges();

        /// <summary>
        /// Checks whether a key is down or not.
        /// </summary>
        public abstract bool IsKeyDown(Keyboard.Key key);
    }
}
