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
using System.Diagnostics;

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
        public static EventModule EventHost { get; private set; }

        /// <summary>
        /// Initializes Square Engine making it's modules available for use
        /// </summary>
        /// <typeparam name="TRenderer"></typeparam>
        public static void Initialize<TRenderer>()
            where TRenderer : IRenderModule, new()
        {
            EventHost = new EventModule();
            Renderer = new TRenderer();
        }

        /// <summary>
        /// Starts the game.
        /// Iniitalize must be called first.
        /// </summary>
        public static void StartGame(string title, int width, int height, WindowStyle style)
        {
            var window = Renderer.CreateWindow(title, width, height, style);
            window.VerticalSynchronization = true;

            EventHost.GetEvent<LoadContentEvent>().Trigger(new LoadContentEvent(null));

            Stopwatch watch = Stopwatch.StartNew();
            while (window.IsOpen)
            {
                float deltaTime = (float)watch.Elapsed.TotalSeconds;
                watch.Restart();
                window.DispatchEvents();

                EventHost.GetEvent<UpdateEvent>().Trigger(new UpdateEvent(null, deltaTime));

                window.Clear(Color.CornflowerBlue);

                EventHost.GetEvent<DrawEvent>().Trigger(new DrawEvent(null, window.RenderTarget));

                window.Display();
            }
        }
    }
}
