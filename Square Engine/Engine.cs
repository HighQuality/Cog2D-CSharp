using Square.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Square.Modules.Renderer;
using Square.Modules.EventHost;

namespace Square
{
    public static class Engine
    {
        public const int VersionNumber = 1;
        public const string EngineVersion = "Square Engine v1 (Pre-Production)";

        /// <summary>
        /// The current render engine
        /// </summary>
        public static IRenderModule Renderer { get; private set; }
        /// <summary>
        /// The current event host
        /// </summary>
        //public static IEventHostModule EventHost { get; private set; }

        /// <summary>
        /// Initializes Square Engine with the specified modules and starts the game
        /// </summary>
        public static void StartGame<TRenderer>(string title, int width, int height, WindowStyle style)
            where TRenderer : IRenderModule, new()
        {
            Renderer = new TRenderer();
            //EventHost = new TEventHost();
            //Renderer.CreateInputEvents(EventHost);

            var window = Renderer.CreateWindow(title, width, height, style);
            window.VerticalSynchronization = true;

            while (window.IsOpen)
            {
                window.DispatchEvents();

                window.Clear(Color.CornflowerBlue);

                window.Display();
            }
        }
    }
}
