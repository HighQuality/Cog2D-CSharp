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
using System.Net.Sockets;
using Square.Modules.Networking;
using System.Threading;

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

        public static ClientModule ClientModule { get; private set; }
        public static ServerModule ServerModule { get; private set; }

        public static bool IsClient { get { return ClientModule != null; } }
        public static bool IsServer { get { return ServerModule != null; } }
        public static bool IsNetworkGame { get { return ClientModule != null || ServerModule != null; } }

        public static float PhysicsTimeStep;

        /// <summary>
        /// Initializes Square Engine making it's modules available for use
        /// </summary>
        /// <typeparam name="TRenderer"></typeparam>
        public static void Initialize<TRenderer>()
            where TRenderer : IRenderModule, new()
        {
            var entireLoadTime = Stopwatch.StartNew();
            random = new Random();
            PhysicsTimeStep = 1f / 120f;

            Debug.Event("Loading Assemblies...");
            Stopwatch watch = Stopwatch.StartNew();
            loadedAssemblies = new Dictionary<string, Assembly>();
            HashSet<string> loaded = new HashSet<string>();
            Stack<AssemblyName> toLoad = new Stack<AssemblyName>(Assembly.GetEntryAssembly().GetReferencedAssemblies());
            var thisAssembly = Assembly.GetExecutingAssembly();
            do
            {
                AssemblyName currentName = toLoad.Pop();
                var assembly = AppDomain.CurrentDomain.Load(currentName);
                loaded.Add(currentName.FullName);
                bool referencesThis = false;
                foreach (var name in assembly.GetReferencedAssemblies())
                {
                    if (name.FullName == thisAssembly.FullName || assembly == thisAssembly)
                        referencesThis = true;
                    if (!loaded.Contains(name.FullName))
                        toLoad.Push(name);
                }
                if (referencesThis)
                {
                    Debug.Info(currentName.Name);
                    loadedAssemblies[currentName.FullName] = assembly;
                }
            }
            while (toLoad.Count > 0);
            Debug.Success("Finished Loading Assemblies! ({0}ms)", watch.Elapsed.TotalMilliseconds);

            // Cache event registrators for Object Components
            ObjectComponent.RegistratorCache = new Dictionary<Type, Action<EventModule, ObjectComponent>>();
            Debug.Event("Pre-Caching Event Registrators...");
            watch.Restart();
            foreach (var assembly in loadedAssemblies.Values)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(ObjectComponent).IsAssignableFrom(type))
                    {
                        Debug.Info(type.FullName);
                        ObjectComponent.CreateRegistrator(type);
                    }
                }
            }
            Debug.Success("Finished Pre-Caching Event Registrators! ({0}ms)", watch.Elapsed.TotalMilliseconds);

            Debug.Event("Registrating Networking Type Handlers...");
            watch.Restart();

            NetworkMessage.RegisterCustomType<Int16>((v, w) => w.Write((Int16)v), r => r.ReadInt16());
            NetworkMessage.RegisterCustomType<UInt16>((v, w) => w.Write((UInt16)v), r => r.ReadUInt16());

            NetworkMessage.RegisterCustomType<Int32>((v, w) => w.Write((Int32)v), r => r.ReadInt32());
            NetworkMessage.RegisterCustomType<UInt32>((v, w) => w.Write((UInt32)v), r => r.ReadUInt32());

            NetworkMessage.RegisterCustomType<Int64>((v, w) => w.Write((Int64)v), r => r.ReadInt64());
            NetworkMessage.RegisterCustomType<UInt64>((v, w) => w.Write((UInt64)v), r => r.ReadUInt64());

            NetworkMessage.RegisterCustomType<Boolean>((v, w) => w.Write((Boolean)v), r => r.ReadBoolean());
            NetworkMessage.RegisterCustomType<Single>((v, w) => w.Write((Single)v), r => r.ReadSingle());
            NetworkMessage.RegisterCustomType<Double>((v, w) => w.Write((Double)v), r => r.ReadDouble());
            NetworkMessage.RegisterCustomType<Decimal>((v, w) => w.Write((Decimal)v), r => r.ReadDecimal());
            NetworkMessage.RegisterCustomType<Char>((v, w) => w.Write((Char)v), r => r.ReadChar());
            NetworkMessage.RegisterCustomType<String>((v, w) => w.Write((String)v), r => r.ReadString());

            NetworkMessage.RegisterCustomType<Byte[]>((v, w) => { w.Write((Int32)v.Length); w.Write(v); }, r => { Int32 size = r.ReadInt32(); return r.ReadBytes(size); });

            Debug.Success("Finished Registrating Networking Type Handlers! ({0}ms)", watch.Elapsed.TotalMilliseconds);

            Debug.Event("Caching Network Events...");
            watch.Restart();
            NetworkMessage.BuildCache(loadedAssemblies.Values);
            Debug.Success("Finished Caching Network Events! ({0}ms)", watch.Elapsed.TotalMilliseconds);

            EventHost = new EventModule();
            SceneHost = new SceneManager();
            Renderer = new TRenderer();

            Debug.Success("Square Engine has been initialized! ({0}ms)", entireLoadTime.Elapsed.TotalMilliseconds);
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
            float accumulator = 0f;
            while (Window.IsOpen)
            {
                float deltaTime = (float)watch.Elapsed.TotalSeconds;
                watch.Restart();
                Window.DispatchEvents();

                accumulator += deltaTime;
                while (accumulator >= PhysicsTimeStep)
                {
                    EventHost.GetEvent<PhysicsUpdateEvent>().Trigger(new PhysicsUpdateEvent(null, PhysicsTimeStep));
                    accumulator -= PhysicsTimeStep;
                }
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

        public static void StartServer(int port)
        {
            ServerModule = new ServerModule(port);

            EventHost.GetEvent<InitializeEvent>().Trigger(new InitializeEvent(null));

            Stopwatch watch = Stopwatch.StartNew();
            float accumulator = 0f;
            while (SceneHost.CurrentScene != null)
            {
                float deltaTime = (float)watch.Elapsed.TotalSeconds;
                watch.Restart();

                accumulator += deltaTime;
                while(accumulator >= PhysicsTimeStep)
                {
                    EventHost.GetEvent<PhysicsUpdateEvent>().Trigger(new PhysicsUpdateEvent(null, PhysicsTimeStep));
                    accumulator -= PhysicsTimeStep;
                }
                EventHost.GetEvent<UpdateEvent>().Trigger(new UpdateEvent(null, deltaTime));
                
                Thread.Sleep(1);
            }

            EventHost.GetEvent<ExitEvent>().Trigger(new ExitEvent(null));
            ServerModule.StopServer();
        }

        /// <summary>
        /// Tries to connect to a Square Engine Server at the given hostname and port.
        /// Returns null if the connection was successfull, otherwise an error message.
        /// </summary>
        public static string ConnectServer(string hostname, int port)
        {
            try
            {
                ClientModule = new Modules.Networking.ClientModule(hostname, port);
            }
            catch(SocketException e)
            {
                return e.Message;
            }

            return null;
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
