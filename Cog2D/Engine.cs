using Cog.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Cog.Modules.Renderer;
using Cog.Modules.EventHost;
using System.Diagnostics;
using Cog.Scenes;
using Cog.Modules.Content;
using System.Net.Sockets;
using Cog.Modules.Networking;
using System.Threading;
using Cog.Modules.Resources;
using Cog.Modules.Animation;
using Cog.Modules.Audio;

namespace Cog
{
    public static class Engine
    {
        public const int VersionNumber = 1;
        public const string EngineVersion = "Cog2D v1 (Pre-Production)";
        private static Random random;
        private static Dictionary<string, Assembly> loadedAssemblies;
        public static Window Window;

        /// <summary>
        /// The current render engine
        /// </summary>
        public static RenderModule Renderer { get; private set; }
        /// <summary>
        /// The current audio engine
        /// </summary>
        public static IAudioModule Audio { get; private set; }
        /// <summary>
        /// Gets the current event host
        /// </summary>
        public static EventModule EventHost { get; private set; }
        /// <summary>
        /// Gets the current scene manager
        /// </summary>
        public static SceneManager SceneHost { get; private set; }
        /// <summary>
        /// Gets the current resource manager
        /// </summary>
        public static ResourceManager ResourceHost { get; private set; }
        /// <summary>
        /// Gets the current host for timed events
        /// </summary>
        internal static TimedEventHost TimedEventHost { get; private set; }
        
        public static ClientModule ClientModule { get; private set; }
        public static ServerModule ServerModule { get; private set; }

        public static bool IsClient { get; private set; }
        public static bool IsServer { get; private set; }
        public static bool IsNetworkGame { get { return ClientModule != null || ServerModule != null; } }

        public static float PhysicsTimeStep;

        private static Dictionary<long, IIdentifier> resolveDictionary;
        private static long nextGlobalId,
            nextLocalId;

        public static Permissions Permissions;
        
        /// <summary>
        /// Gets or sets the desired resolution.
        /// If a window has already been created the renderer must be restarted for a new resolution to take affect.
        /// </summary>
        public static Vector2 DesiredResolution { get; set; }
        /// <summary>
        /// If there is no window currently created, gets the desired resolution.
        /// Otherwise gets the window's actual resolution.
        /// </summary>
        public static Vector2 Resolution { get { if (Window != null) return Window.Resolution; return DesiredResolution; } }

        public static double TimeStamp { get; private set; }
        private static object _synchronizedTimeStampLock;
        private static double _synchronizedTimeStamp;
        public static double SynchronizedTimeStamp { get { lock (_synchronizedTimeStampLock) return _synchronizedTimeStamp; } private set { lock (_synchronizedTimeStampLock) _synchronizedTimeStamp = value; } }
        private static Stopwatch timeStampWatch;

        public static float FrameTime { get; private set; }

        /// <summary>
        /// Initializes Cog2D making it available for use showing the default splash screen while initializing
        /// </summary>
        /// <typeparam name="TRenderer">The renderer to use</typeparam>
        public static void Initialize<TRenderer, TAudioModule>()
            where TRenderer : RenderModule, new()
            where TAudioModule : IAudioModule, new()
        {
            InnerInitialize<TRenderer, TAudioModule>(new Image("splash.png"));
        }

        /// <summary>
        /// Initializes Cog2D making it available for use
        /// </summary>
        /// <typeparam name="TRenderer">The renderer to use</typeparam>
        /// <param name="splashScreenImage">The splash screen to show while initializing. Pass null for no splash screen.</param>
        public static void Initialize<TRenderer, TAudioModule>(Image splashScreenImage)
            where TRenderer : RenderModule, new()
            where TAudioModule : IAudioModule, new()
        {
            InnerInitialize<TRenderer, TAudioModule>(splashScreenImage);
        }

        private static void InnerInitialize<TRenderer, TAudioModule>(Image splashScreenImage)
            where TRenderer : RenderModule, new()
            where TAudioModule : IAudioModule, new()
        {
            EventHost = new EventModule();
            TimedEventHost = new TimedEventHost();

            _synchronizedTimeStampLock = new object();

            if (splashScreenImage != null)
            {
                AutoResetEvent ev = new AutoResetEvent(false);
                SplashScreen splashScreen = null;
                EventHost.RegisterEvent<FinishedLoadingEvent>(-999, e => splashScreen.Invoke((System.Windows.Forms.MethodInvoker)delegate { splashScreen.Close(); }));

                var thread = new Thread(() =>
                {
                    splashScreen = new SplashScreen(splashScreenImage);
                    ev.Set();
                    splashScreen.ShowDialog();
                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.IsBackground = true;
                thread.Start();

                ev.WaitOne();
            }

            var entireLoadTime = Stopwatch.StartNew();
            random = new Random();
            PhysicsTimeStep = 1f / 120f;
            Permissions = Permissions.FullPermissions;

            nextGlobalId = 1;
            nextLocalId = -1;
            resolveDictionary = new Dictionary<long, IIdentifier>();

            DesiredResolution = new Vector2(640f, 480f);

            NetworkMessage.InitializeCache();
            GameObject.InitializeCache();
            SceneCache.InitializeCache();
            Mouse.Initialize();
            AnimationInterpolations.Initialize();

            Debug.Event("Loading Assemblies...");
            Stopwatch watch = Stopwatch.StartNew();
            loadedAssemblies = new Dictionary<string, Assembly>();
            HashSet<string> loaded = new HashSet<string>();
            var entryAssembly = Assembly.GetEntryAssembly();
            var refAssemblies = entryAssembly.GetReferencedAssemblies();
            Stack<AssemblyName> toLoad = new Stack<AssemblyName>(refAssemblies);
            toLoad.Push(entryAssembly.GetName());
            loaded.Add(entryAssembly.FullName);
            foreach (var assembly in refAssemblies)
                loaded.Add(assembly.FullName);
            var thisAssembly = Assembly.GetExecutingAssembly();

            do
            {
                AssemblyName currentName = toLoad.Pop();
                var assembly = AppDomain.CurrentDomain.Load(currentName);

                bool referencesThis = false;
                foreach (var name in assembly.GetReferencedAssemblies())
                {
                    if (name.FullName == thisAssembly.FullName || assembly == thisAssembly)
                        referencesThis = true;
                    if (!loaded.Contains(name.FullName))
                    {
                        toLoad.Push(name);
                        loaded.Add(name.FullName);
                    }
                }
                if (referencesThis)
                {
                    loadedAssemblies[currentName.FullName] = assembly;
                }
            }
            while (toLoad.Count > 0);

            Debug.Success("Discovered {0} Content Assemblies! ({1}ms)", loadedAssemblies.Count, watch.Elapsed.TotalMilliseconds);

            Debug.Event("Registrating Type Serializers...");
            watch.Restart();

            // UPDATE DOCUMENTATION UNDER "NETWORKING" WHEN MAKING CHANGES
            TypeSerializer.Register<Int16>((r, w) => w.Write(r.ReadBytes(sizeof(Int16))), (v, w) => w.Write((Int16)v), r => r.ReadInt16());
            TypeSerializer.Register<UInt16>((r, w) => w.Write(r.ReadBytes(sizeof(UInt16))), (v, w) => w.Write((UInt16)v), r => r.ReadUInt16());

            TypeSerializer.Register<Int32>((r, w) => w.Write(r.ReadBytes(sizeof(Int32))), (v, w) => w.Write((Int32)v), r => r.ReadInt32());
            TypeSerializer.Register<UInt32>((r, w) => w.Write(r.ReadBytes(sizeof(UInt32))), (v, w) => w.Write((UInt32)v), r => r.ReadUInt32());

            TypeSerializer.Register<Int64>((r, w) => w.Write(r.ReadBytes(sizeof(Int64))), (v, w) => w.Write((Int64)v), r => r.ReadInt64());
            TypeSerializer.Register<UInt64>((r, w) => w.Write(r.ReadBytes(sizeof(UInt64))), (v, w) => w.Write((UInt64)v), r => r.ReadUInt64());

            TypeSerializer.Register<Byte>((r, w) => w.Write(r.ReadBytes(sizeof(Byte))), (v, w) => w.Write((Byte)v), r => r.ReadByte());
            TypeSerializer.Register<SByte>((r, w) => w.Write(r.ReadBytes(sizeof(SByte))), (v, w) => w.Write((SByte)v), r => r.ReadSByte());

            TypeSerializer.Register<Boolean>((r, w) => w.Write(r.ReadBytes(sizeof(Boolean))), (v, w) => w.Write((Boolean)v), r => r.ReadBoolean());
            TypeSerializer.Register<Single>((r, w) => w.Write(r.ReadBytes(sizeof(Single))), (v, w) => w.Write((Single)v), r => r.ReadSingle());
            TypeSerializer.Register<Double>((r, w) => w.Write(r.ReadBytes(sizeof(Double))), (v, w) => w.Write((Double)v), r => r.ReadDouble());
            TypeSerializer.Register<Decimal>((r, w) => w.Write(r.ReadBytes(sizeof(Decimal))), (v, w) => w.Write((Decimal)v), r => r.ReadDecimal());
            TypeSerializer.Register<Char>((r, w) => w.Write(r.ReadBytes(sizeof(Char))), (v, w) => w.Write((Char)v), r => r.ReadChar());
            TypeSerializer.Register<String>((r, w) =>
            {
                UInt32 size = r.ReadUInt32();
                w.Write((UInt32)size);
                if (size != 0)
                    w.Write(r.ReadBytes((int)size));
            }, (v, w) =>
            {
                if (v == null || v.Length == 0)
                    w.Write((UInt32)0);
                else
                {
                    var bytes = Encoding.UTF8.GetBytes(v);
                    w.Write((UInt32)bytes.Length);
                    w.Write(bytes);
                }
            }, r =>
            {
                UInt32 size = r.ReadUInt32();
                if (size == 0)
                    return "";
                return Encoding.UTF8.GetString(r.ReadBytes((int)size));
            });

            TypeSerializer.Register<Guid>((r, w) => w.Write(r.ReadBytes(16)), (v, w) => w.Write(v.ToByteArray()), r => new Guid(r.ReadBytes(16)));

            TypeSerializer.Register<Vector2>((r, w) => w.Write(r.ReadBytes(sizeof(float) * 2)), (v, w) => { w.Write((float)v.X); w.Write((float)v.Y); }, r => { Vector2 v; v.X = r.ReadSingle(); v.Y = r.ReadSingle(); return v; });
            TypeSerializer.Register<Rectangle>((r, w) => w.Write(r.ReadBytes(sizeof(float) * 4)), (v, w) => { w.Write((float)v.TopLeft.X); w.Write((float)v.TopLeft.Y); w.Write((float)v.Size.X); w.Write((float)v.Size.Y); }, r => { Vector2 topLeft, size; topLeft.X = r.ReadSingle(); topLeft.Y = r.ReadSingle(); size.X = r.ReadSingle(); size.Y = r.ReadSingle(); return new Rectangle(topLeft, size); });
            TypeSerializer.Register<Color>((r, w) => w.Write(r.ReadBytes(sizeof(byte) * 4)), (v, w) => { w.Write((byte)v.R); w.Write((byte)v.G); w.Write((byte)v.B); w.Write((byte)v.A); }, r => { Color v; v.R = r.ReadByte(); v.G = r.ReadByte(); v.B = r.ReadByte(); v.A = r.ReadByte(); return v; });

            TypeSerializer.Register<GameObject>((r, w) => w.Write(r.ReadBytes(sizeof(Int64))), (v, w) => { if (v == null) w.Write((long)0); else w.Write((long)v.Id); }, r =>
            {
                var id = r.ReadInt64();
                if (id == 0)
                    return null;
                return Engine.Resolve<GameObject>(id);
            });

            TypeSerializer.Register<Scene>((r, w) => w.Write(r.ReadBytes(sizeof(Int64))), (v, w) => { if (v == null) w.Write((long)0); else w.Write((long)v.Id); }, r =>
            {
                var id = r.ReadInt64();
                if (id == 0)
                    return null;
                return Engine.Resolve<Scene>(id);
            });

            Debug.Success("Finished Registrating Type Serializers! ({0}ms)", watch.Elapsed.TotalMilliseconds);

            // Cache event registrators for Object Components
            Debug.Event("Generating Game Meta Data...");
            watch.Restart();
            {
                List<Action> jobQueue = new List<Action>();

                foreach (var assembly in loadedAssemblies.Values.OrderBy(o => o.FullName))
                {
                    foreach (var type in assembly.GetTypes().OrderBy(o => o.FullName))
                    {
                        if (typeof(GameObject).IsAssignableFrom(type))
                        {
                            if (!type.IsAbstract)
                                jobQueue.Add(() => GameObject.CreateCache(type));
                            TypeSerializer.RegisterDescendant<GameObject>(type);
                        }
                        else if (typeof(Scene).IsAssignableFrom(type))
                        {
                            if (!type.IsAbstract)
                                jobQueue.Add(() => SceneCache.CreateCache(type));
                            TypeSerializer.RegisterDescendant<Scene>(type);
                        }
                        else if (typeof(NetworkMessage).IsAssignableFrom(type))
                        {
                            if (!type.IsAbstract)
                                jobQueue.Add(() => NetworkMessage.CreateCache(type));
                        }
                    }
                }

                for (int i = 0; i < jobQueue.Count; i++)
                    jobQueue[i]();
            }

            StringBuilder hash = new StringBuilder();
            hash.Append(NetworkMessage.GetNetworkDescriber());
            hash.Append(GameObject.GetNetworkDescriber());

            // Generates a SHA256-hash from the string describing the networking classes
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                NetworkMessage.NetworkingHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(hash.ToString()));
                // Debug.Info("Generated Networking Hash:\n{0}", BitConverter.ToString(NetworkMessage.NetworkingHash).Replace("-", ""));
            }

            Debug.Success("Finished Generating Meta Data! ({0}ms)", watch.Elapsed.TotalMilliseconds);

            Debug.Event("Initializing Core Modules...");
            watch.Restart();
            ResourceHost = new ResourceManager();
            SceneHost = new SceneManager();
            Renderer = new TRenderer();
            Audio = new TAudioModule();
            Debug.Success("Finished Initializing Core Modules! ({0}ms)", watch.Elapsed.TotalMilliseconds);

            Debug.Success("Cog2D has been initialized! ({0}ms)", entireLoadTime.Elapsed.TotalMilliseconds);
            Debug.NewLine();
        }

        /// <summary>
        /// Starts the game.
        /// Iniitalize must be called first.
        /// </summary>
        public static void StartGame(string title, WindowStyle style)
        {
            IsServer = false;
            // You're only a client if you're connected to a server
            IsClient = false;

            Window = Renderer.CreateWindow(title, (int)DesiredResolution.X, (int)DesiredResolution.Y, style);
            // Window.VerticalSynchronization = true;
            Renderer.CreateBlendModes();

            EventHost.GetEvent<InitializeEvent>().Trigger(new InitializeEvent(null));
            EventHost.RegisterEvent<ExitEvent>(-999, e => { Window.Close(); });
            EventHost.RegisterEvent<CloseButtonEvent>(-999, e => { EventHost.GetEvent<ExitEvent>().Trigger(new ExitEvent(null)); e.Intercept = true; });

            Window.Visible = true;

            EventHost.GetEvent<FinishedLoadingEvent>().Trigger(new FinishedLoadingEvent(null));

            timeStampWatch = Stopwatch.StartNew();
            Stopwatch watch = Stopwatch.StartNew();
            float accumulator = 0f;
            while (Window.IsOpen)
            {
                TimeStamp = timeStampWatch.Elapsed.TotalSeconds;
                SynchronizedTimeStamp = TimeStamp;
                float deltaTime = (float)watch.Elapsed.TotalSeconds;
                watch.Restart();

                lock (TimedEventHost)
                    TimedEventHost.Update();

                Window.DispatchEvents();

                accumulator += deltaTime;
                while (accumulator >= PhysicsTimeStep)
                {
                    EventHost.GetEvent<PhysicsUpdateEvent>().Trigger(new PhysicsUpdateEvent(null, PhysicsTimeStep));
                    accumulator -= PhysicsTimeStep;
                }
                EventHost.GetEvent<UpdateEvent>().Trigger(new UpdateEvent(null, deltaTime));

                EventHost.GetEvent<BeginDrawEvent>().Trigger(new BeginDrawEvent(null, Window.RenderTarget));

                if (SceneHost.CurrentScene == null)
                    Window.Clear(Color.White);
                else
                    Window.Clear(SceneHost.CurrentScene.BackgroundColor);

                EventHost.GetEvent<DrawEvent>().Trigger(new DrawEvent(null, Window.RenderTarget));

                Engine.Window.RenderTarget.SetTransformation(Engine.Resolution / 2f, Vector2.One, Angle.FromDegree(0f));
                EventHost.GetEvent<DrawInterfaceEvent>().Trigger(new DrawInterfaceEvent(null, Window.RenderTarget));

                float frameTime = (float)watch.Elapsed.TotalSeconds;
                Engine.FrameTime = frameTime;
                Window.Display();

                if (SceneHost.CurrentScene == null)
                    Window.Close();
            }
        }

        public static void StartServer(int port, Func<TcpClient, CogClient> createClient)
        {
            IsServer = true;
            IsClient = false;

            EventHost.GetEvent<InitializeEvent>().Trigger(new InitializeEvent(null));

            ServerModule = new ServerModule(port, createClient);

            EventHost.GetEvent<FinishedLoadingEvent>().Trigger(new FinishedLoadingEvent(null));

            timeStampWatch = Stopwatch.StartNew();
            Stopwatch watch = Stopwatch.StartNew();
            float accumulator = 0f;
            while (SceneHost.CurrentScene != null)
            {
                TimeStamp = timeStampWatch.Elapsed.TotalSeconds;
                SynchronizedTimeStamp = TimeStamp;
                float deltaTime = (float)watch.Elapsed.TotalSeconds;
                watch.Restart();

                lock (TimedEventHost)
                    TimedEventHost.Update();

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
        /// Tries to connect to a Cog2D Server at the given hostname and port.
        /// Returns null if the connection was successfull, otherwise an error message.
        /// </summary>
        public static string ConnectServer(string hostname, int port)
        {
            try
            {
                IsClient = true;

                ClientModule = new Modules.Networking.ClientModule(hostname, port);
            }
            catch(SocketException e)
            {
                return e.Message;
            }

            return null;
        }

        /// <summary>
        /// Invokes the given action synchronized, in the main thread, after the given time has elapsed.
        /// Passes the time in seconds it missed by.
        /// </summary>
        public static TimedEvent InvokeTimed(float timeInSeconds, Action<float> action)
        {
            var ev = new TimedEvent(action, SynchronizedTimeStamp + (double)timeInSeconds);
            lock (TimedEventHost)
                TimedEventHost.Schedule(ev);
            return ev;
        }

        /// <summary>
        /// Returns a random floating-point value ranging from 0 - 1
        /// </summary>
        /// <returns>A random value in the range of 0 - 1</returns>
        public static float RandomFloat()
        {
            return (float)random.NextDouble();
        }

        public static T Roll<T>(params T[] options)
        {
            return options[random.Next(options.Length)];
        }

        internal static void GenerateGlobalId(IIdentifier obj)
        {
            if (IsClient && !IsServer)
                throw new InvalidOperationException("Only the server may generate global IDs!");
            if (obj == null)
                throw new ArgumentNullException("gameObject");
            if (obj.Id != 0)
                throw new InvalidOperationException("GameObject has already been assigned an ID!");
            obj.Id = nextGlobalId++;
            resolveDictionary.Add(obj.Id, obj);
        }

        internal static void GenerateLocalId(IIdentifier obj)
        {
            if (obj == null)
                throw new ArgumentNullException("gameObject");
            if (obj.Id != 0)
                throw new InvalidOperationException("GameObject has already been assigned an ID!");
            obj.Id = nextLocalId--;
            resolveDictionary.Add(obj.Id, obj);
        }

        internal static void AssignId(IIdentifier obj, long id)
        {
            if (!IsNetworkGame)
                throw new InvalidOperationException("Only games using networking may use Engine.AssignId()");
            if (IsServer)
                throw new InvalidOperationException("Only the client may use Engine.AssignID()");
            if (resolveDictionary.ContainsKey(id))
                throw new InvalidOperationException("ID {0} is already in use!");
            obj.Id = id;
            resolveDictionary.Add(id, obj);
        }

        public static T Resolve<T>(long id)
            where T : IIdentifier
        {
            IIdentifier obj;
            resolveDictionary.TryGetValue(id, out obj);
            if (obj is T || obj == null)
                return (T)obj;
            throw new Exception(string.Format("{0} with ID {1} is not a {2}", obj.GetType().Name, id, typeof(T).Name));
        }
    }
}
