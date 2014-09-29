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
using Square.Scenes;
using Square.Modules.Content;

namespace Square
{
    public static class Engine
    {
        public const int VersionNumber = 1;
        public const string EngineVersion = "Square Engine v1 (Pre-Production)";
        private static Random random;
        private static Dictionary<string, Assembly> loadedAssemblies;
        internal static IWindow Window;

        /// <summary>
        /// The current render engine
        /// </summary>
        public static IRenderModule Renderer { get; private set; }
        /// <summary>
        /// The current event host
        /// </summary>
        public static EventModule EventHost { get; private set; }
        /// <summary>
        /// The current scene manager
        /// </summary>
        public static SceneManager SceneHost { get; private set; }

        /// <summary>
        /// Initializes Square Engine making it's modules available for use
        /// </summary>
        /// <typeparam name="TRenderer"></typeparam>
        public static void Initialize<TRenderer>()
            where TRenderer : IRenderModule, new()
        {
            random = new Random();

            Debug.Info("Discovering Assemblies...");
            Stopwatch watch = Stopwatch.StartNew();
            loadedAssemblies = new Dictionary<string, Assembly>();
            var thisAssembly = Assembly.GetExecutingAssembly();
            var thisAssemblyName = thisAssembly.GetName();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // Add assemblies that reference this assembly for caching
                if (assembly.GetReferencedAssemblies().Where(o => o.FullName == thisAssembly.FullName).FirstOrDefault() != null || assembly == thisAssembly)
                {
                    Console.WriteLine("Game Assembly \"" + assembly.GetName().Name + "\" discovered!");
                    loadedAssemblies.Add(assembly.FullName, assembly);
                }
            }
            Debug.Success("Discovered {0} Assemblies in {1}ms!", loadedAssemblies.Count, watch.Elapsed.TotalMilliseconds);
            
            // Cache event registrators for Object Components
            ObjectComponent.RegistratorCache = new Dictionary<Type, Action<EventModule, ObjectComponent>>();
            Debug.Info("Pre-Caching Event Registrators...");
            watch.Restart();
            foreach (var assembly in loadedAssemblies.Values)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(ObjectComponent).IsAssignableFrom(type))
                    {
                        Console.WriteLine(type.FullName);
                        ObjectComponent.CreateRegistrator(type);
                    }
                }
            }
            Debug.Success("Finished Pre-Caching Event Registrators! ({0}ms)", watch.Elapsed.TotalMilliseconds);

            EventHost = new EventModule();
            SceneHost = new SceneManager();
            Renderer = new TRenderer();
        }

        /// <summary>
        /// Starts the game.
        /// Iniitalize must be called first.
        /// </summary>
        public static void StartGame(string title, int width, int height, WindowStyle style)
        {
            Window = Renderer.CreateWindow(title, width, height, style, EventHost);
            Window.VerticalSynchronization = true;

            EventHost.GetEvent<InitializeEvent>().Trigger(new InitializeEvent(null));
            EventHost.RegisterEvent<ExitEvent>(-999, e => { Window.Close(); e.Intercept = true; });
            EventHost.RegisterEvent<CloseButtonEvent>(-999, e => { EventHost.GetEvent<ExitEvent>().Trigger(new ExitEvent(null)); e.Intercept = true; });

            Stopwatch watch = Stopwatch.StartNew();
            while (Window.IsOpen)
            {
                float deltaTime = (float)watch.Elapsed.TotalSeconds;
                watch.Restart();
                Window.DispatchEvents();

                EventHost.GetEvent<UpdateEvent>().Trigger(new UpdateEvent(null, deltaTime));

                Window.Clear(Color.CornflowerBlue);

                EventHost.GetEvent<DrawEvent>().Trigger(new DrawEvent(null, Window.RenderTarget));
                EventHost.GetEvent<DrawInterfaceEvent>().Trigger(new DrawInterfaceEvent(null, Window.RenderTarget));

                float frameTime = (float)watch.Elapsed.TotalMilliseconds;
                Window.Title = frameTime.ToString();
                Window.Display();

                if (SceneHost.CurrentScene == null)
                    Window.Close();
            }
        }

        /// <summary>
        /// Returns a random floating-point value ranging from 0 - 1
        /// </summary>
        /// <returns>A random value in the range of 0 - 1</returns>
        public static float RandomFloat()
        {
            return (float)random.NextDouble();
        }
    }
}
