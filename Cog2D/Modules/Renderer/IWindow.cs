using Cog.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Renderer
{
    public interface IWindow
    {
        /// <summary>
        /// Gets or Sets the title of the window
        /// </summary>
        string Title { get; set; }
        /// <summary>
        /// Gets or Sets the size of the window
        /// </summary>
        Vector2 Resolution { get; set; }
        /// <summary>
        /// Gets or Sets the position of the window
        /// </summary>
        Vector2 Position { get; set; }
        /// <summary>
        /// Gets or Sets the visibility of the window
        /// </summary>
        bool Visible { get; set; }
        /// <summary>
        /// Gets if the window is still open
        /// </summary>
        bool IsOpen { get; }
        bool VerticalSynchronization { get; set; }

        /// <summary>
        /// Gets the current render target
        /// </summary>
        IRenderTarget RenderTarget { get; }

        /// <summary>
        /// Commands the window to dispatch it's queued events
        /// </summary>
        void DispatchEvents();
        /// <summary>
        /// Commands the window to clear the back buffer to the specified color
        /// </summary>
        /// <param name="color">The color to clear to</param>
        void Clear(Color color);
        /// <summary>
        /// Commands the window to switch the backbuffer displaying rendered content
        /// </summary>
        void Display();
        /// <summary>
        /// Closes the window
        /// </summary>
        void Close();

        /// <summary>
        /// Commands the window to apply changes.
        /// Depending on the renderer, changes may be applied before this method is called.
        /// </summary>
        void ApplyChanges();

        /// <summary>
        /// Checks whether a key is down or not.
        /// </summary>
        bool IsKeyDown(Keyboard.Key key);
    }
}
