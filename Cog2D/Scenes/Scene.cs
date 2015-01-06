﻿using Cog.Modules;
using Cog.Modules.Content;
using Cog.Modules.EventHost;
using Cog.Modules.Renderer;
using Cog.Interface;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Cog.Modules.Resources;
using System.IO;
using Cog.Modules.Networking;

namespace Cog.Scenes
{
    public abstract class Scene : IIdentifier
    {
        public string Name { get; private set; }
        public int SceneId { get; private set; }
        /// <summary>
        /// Whether or not events in this scene should be triggered when this scene is not current
        /// </summary>
        public bool SimulateInBackground { get; set; }
        public LinkedList<GameObject> Objects = new LinkedList<GameObject>();
        public EventModule EventModule = new EventModule();
        private Dictionary<EventIdentifier, List<IEventListener>> eventStrength = new Dictionary<EventIdentifier, List<IEventListener>>();
        private Dictionary<EventIdentifier, IEventListener> globalListeners = new Dictionary<EventIdentifier, IEventListener>();
        private List<EventIdentifier> toBeRemoved = new List<EventIdentifier>();
        private float eventStrengthUpdateTimer = 0f;
        public GameInterface Interface;
        private List<Resource> loadedResources = new List<Resource>();

        public Color BackgroundColor = Color.CornflowerBlue;

        internal LinkedList<GameObject> BaseObjects = new LinkedList<GameObject>();

        private Camera _camera;
        public Camera Camera
        {
            get { return _camera; }
            set
            {
                if (_camera != null)
                    _camera.Unbind();
                _camera = value;
                if (_camera != null)
                    _camera.Bind();
            }
        }

        internal Dictionary<DrawCell, HashSet<GameObject>> DrawCells = new Dictionary<DrawCell, HashSet<GameObject>>();
        internal Queue<DrawCellMoveInfo> DrawCellMoveQueue = new Queue<DrawCellMoveInfo>();

        private List<CogClient> subscribedClients = new List<CogClient>();

        public long Id { get; set; }
        public bool IsGlobal { get { return Id > 0; } }
        public bool IsLocal { get { return Id < 0; } }

        public Scene(string name)
        {
            if (Id == 0)
                throw new InvalidOperationException("No ID was assigned to the scene!\nDid you create the scene through SceneManager.Create<T>()?");

            this.Name = name;
            SceneId = SceneCache.IdFromType(GetType());

            // Randomize an offset for the event strength update timer
            eventStrengthUpdateTimer = Engine.RandomFloat();

            Interface = new GameInterface();
            Interface.Size = Engine.Resolution;

            RegisterEvent<UpdateEvent>(0, e => { Update(e); Interface.TriggerUpdate(e); });
            RegisterEvent<BeginDrawEvent>(1000, BeginDraw);
            RegisterEvent<DrawEvent>(0, e => { Draw(e); });
            RegisterEvent<DrawInterfaceEvent>(0, e => { Interface.TriggerDraw(e, new Vector2()); });
            
            Camera = CreateLocalObject<Camera>(new Vector2());
        }

        private void Update(UpdateEvent args)
        {
            {
                var current = Objects.First;
                while (current != null)
                {
                    if (current.Value.DoRemove)
                        Objects.Remove(current);
                    current = current.Next;
                }
            }

            eventStrengthUpdateTimer += args.DeltaTime;
            while (eventStrengthUpdateTimer >= 1f)
            {
                foreach (var pair in eventStrength)
                {
                    var list = pair.Value;

                    for (int i = list.Count - 1; i >= 0; i--)
                        if (list[i].IsCancelled)
                            list.RemoveAt(i);

                    if (list.Count == 0)
                        toBeRemoved.Add(pair.Key);
                }

                for (int i = toBeRemoved.Count - 1; i >= 0; i--)
                {
                    Debug.Event("Stopped listening for " + globalListeners[toBeRemoved[i]].IEvent.GetType().GetGenericArguments()[0].Name + " events");

                    // Cancel and remove the global listener (Engine.EventModule -> this scene's event module)
                    globalListeners[toBeRemoved[i]].Cancel();
                    globalListeners.Remove(toBeRemoved[i]);

                    // No longer monitor this event strength
                    eventStrength.Remove(toBeRemoved[i]);
                }
                toBeRemoved.Clear();

                eventStrengthUpdateTimer -= 1f;
            }
        }
        
        public void BeginDraw(BeginDrawEvent ev)
        {
            while (DrawCellMoveQueue.Count > 0)
            {
                var info = DrawCellMoveQueue.Dequeue();
                var obj = info.Object;

                if (!obj.DoRemove)
                {
                    // If we're eligible, our parent may have changed since we first enqueued
                    if (obj.Parent == null)
                    {
                        if (!info.IsInitialPlacement)
                        {
                            // Remove our old entry
                            var currentSet = DrawCells[obj.CurrentDrawCell];
                            currentSet.Remove(obj);
                            if (currentSet.Count == 0)
                                DrawCells.Remove(obj.CurrentDrawCell);
                        }

                        obj.CurrentDrawCell = new DrawCell((int)Math.Floor(obj.LocalCoord.X / (float)DrawCell.DrawCellSize), (int)Math.Floor(obj.LocalCoord.Y / (float)DrawCell.DrawCellSize));

                        HashSet<GameObject> newSet;
                        if (!DrawCells.TryGetValue(obj.CurrentDrawCell, out newSet))
                        {
                            newSet = new HashSet<GameObject>();
                            DrawCells.Add(obj.CurrentDrawCell, newSet);
                        }
                        newSet.Add(obj);
                    }
                }

                obj.IsScheduledForDrawCellMove = false;
            }
        }

        public void Draw(DrawEvent ev)
        {
            DrawTransformation transform = new DrawTransformation();
            transform.WorldScale = Vector2.One;
            transform.ParentWorldScale = Vector2.One;

            Vector2 cameraSize = Engine.Resolution * Camera.WorldScale;
            Vector2 cameraPosition = Camera.WorldCoord - cameraSize / 2f;
            int x1 = (int)Math.Floor(cameraPosition.X / (float)DrawCell.DrawCellSize),
                y1 = (int)Math.Floor(cameraPosition.Y / (float)DrawCell.DrawCellSize),
                x2 = x1 + (int)Math.Ceiling(cameraSize.X / (float)DrawCell.DrawCellSize),
                y2 = y1 + (int)Math.Ceiling(cameraSize.Y / (float)DrawCell.DrawCellSize);

            List<Tuple<float, Action>> drawList = new List<Tuple<float, Action>>();

            for (int x = x1; x <= x2; x++)
            {
                for (int y = y1; y <= y2; y++)
                {
                    HashSet<GameObject> objectSet;
                    DrawCell cell;
                    cell.X = x;
                    cell.Y = y;

                    if (DrawCells.TryGetValue(cell, out objectSet))
                        foreach (var obj in objectSet)
                            obj.Draw(ev, transform, drawList);
                }
            }

            foreach (var obj in drawList.OrderBy(o => -o.Item1))
                obj.Item2();
        }

        internal SceneCreationMessage CreateSceneCreationMessage()
        {
            if (IsLocal)
                throw new InvalidOperationException("Tried to create a scene creation message from a local scene!");

            var msg = new SceneCreationMessage();
            msg.SceneName = Name;
            msg.TypeId = (ushort)SceneId;
            msg.Id = Id;

            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    var objects = Objects.Where(o => o.IsGlobal).ToArray();
                    writer.Write((UInt32)objects.Length);

                    for (int i = 0; i < objects.Length; i++)
                    {
                        writer.Write((ushort)GameObject.IdFromType(objects[i].GetType()));
                        writer.Write((long)objects[i].Id);
                    }

                    for (int i = 0; i < objects.Length; i++)
                    {
                        var obj = objects[i];
                        obj.Serialize(writer);
                    }

                    msg.Data = stream.ToArray();
                }
            }

            return msg;
        }

        internal void ReadSceneCreationData(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    var objCount = reader.ReadUInt32();
                    var objects = new GameObject[objCount];

                    for (int i = 0; i < objects.Length; i++)
                    {
                        ushort typeId = reader.ReadUInt16();
                        long id = reader.ReadInt64();
                        objects[i] = CreateUninitializedObject(GameObject.TypeFromId(typeId), null);
                        Engine.AssignId(objects[i], id);
                    }

                    for (int i = 0; i < objects.Length; i++)
                        objects[i].Deserialize(reader);
                    for (int i = 0; i < objects.Length; i++)
                        InitializeObject(objects[i]);
                    for (int i = 0; i < objects.Length; i++)
                        objects[i].Initialize();
                }
            }
        }

        public T CreateObject<T>(Vector2 localCord)
            where T : GameObject, new()
        {
            return CreateObject<T>(null, null, localCord);
        }

        public T CreateObject<T>(GameObject parent, Vector2 localCord)
            where T : GameObject, new()
        {
            return CreateObject<T>(null, parent, localCord);
        }

        public T CreateObject<T>(CogClient owner, GameObject parent, Vector2 localCoord)
            where T : GameObject, new()
        {
            if (Engine.IsClient)
                throw new Exception("Can not create global objects when connected to a server!");

            T obj = (T)CreateUninitializedObject(typeof(T), parent);
            Engine.GenerateGlobalId(obj);
            obj.LocalCoord = localCoord;

            // Engine/Object Constructor
            InitializeObject(obj);
            // Object Initialization
            obj.Initialize();

            return obj;
        }

        public GameObject CreateUninitializedObject(Type type, GameObject parent)
        {
            // Create an object of this type without invoking the constructor
            GameObject obj = (GameObject)FormatterServices.GetUninitializedObject(type);
            obj.Scene = this;
            obj.InitialSetParent(parent);

            InitializationData data = new InitializationData();
            var synchronizedFields = GameObject.GetSynchronizedFields(type);
            data.SynchronizedFields = new ISynchronized[synchronizedFields.Length - 1];

            for (int i = 1; i < synchronizedFields.Length; i++)
            {
                data.SynchronizedFields[i - 1] = (ISynchronized)Activator.CreateInstance(synchronizedFields[i].FieldType, true);
                data.SynchronizedFields[i - 1].Initialize(obj, (ushort)i);
            }
            
            obj.InitializationData = data;

            if (parent == null)
            {
                obj.IsScheduledForDrawCellMove = true;
                DrawCellMoveQueue.Enqueue(new DrawCellMoveInfo
                {
                    Object = obj,
                    IsInitialPlacement = true
                });
            }

            // Register the new object
            Objects.AddLast(obj);
            
            return obj;
        }

        public void InitializeObject(GameObject obj)
        {
            // Invoke the constructor
            obj.GetType().GetConstructor(new Type[0]).Invoke(obj, new object[0]);

            Engine.EventHost.GetEvent<ObjectCreated>().Trigger(new ObjectCreated(this, obj));

            if (Engine.IsServer && obj.IsGlobal)
            {
                var msg = obj.CreateCreationMessage();
                foreach (var client in EnumerateSubscribedClients())
                {
                    client.Send(msg);
                    obj.SubscribedClients.Add(client);
                }
            }
        }

        public T CreateLocalObject<T>(Vector2 localCoord)
            where T : GameObject, new()
        {
            return CreateLocalObject<T>(null, localCoord);
        }

        public T CreateLocalObject<T>(GameObject parent, Vector2 localCoord)
            where T : GameObject, new()
        {
            T obj = (T)CreateUninitializedObject(typeof(T), parent);
            Engine.GenerateLocalId(obj);
            obj.LocalCoord = localCoord;

            // Engine/object constructor
            InitializeObject(obj);
            // Object initialization
            obj.Initialize();

            return obj;
        }

        public void Clear()
        {
            var current = Objects.First;
            do
            {
                if (!current.Value.DoRemove)
                    current.Value.Remove();
            }
            while (current != null);

            Objects.Clear();
        }

        public void Preload<T>()
            where T : GameObject
        {
            foreach (var attribute in typeof(T).GetCustomAttributes(typeof(ResourceAttribute), true).Select(o => (ResourceAttribute)o))
            {
                //var container = Engine.GetResourceContainer(attribute.ContainerName);
            }
        }

        public EventListener<T> RegisterEvent<T>(int priority, Action<T> action)
            where T : EventParameters
        {
            return RegisterEvent<T>(null, priority, action);
        }

        public EventListener<T> RegisterEvent<T>(object uniqueIdentifier, int priority, Action<T> action)
            where T : EventParameters
        {
            var listener = EventModule.RegisterEvent<T>(uniqueIdentifier, priority, (ev) =>
            {
                if (Engine.SceneHost.CurrentScene == this || SimulateInBackground)
                    action(ev);
            });
            AddEventStrength<T>(listener);
            return listener;
        }

        public void AddEventStrength<T>(EventListener<T> listener)
            where T : EventParameters
        {
            List<IEventListener> listenerList;

            if (!eventStrength.TryGetValue(listener.Event.Identifier, out listenerList))
            {
                listenerList = new List<IEventListener>();
                eventStrength.Add(listener.Event.Identifier, listenerList);

                // TODO: Move event registration to SceneManager
                globalListeners.Add(listener.Event.Identifier, Engine.EventHost.RegisterEvent<T>(listener.Event.Identifier.UniqueIdentifier, 0, e => { if (Engine.SceneHost.CurrentScene == this) EventModule.GetEvent<T>(listener.Event.Identifier.UniqueIdentifier).Trigger(e); }));
            }

            listenerList.Add(listener);
        }

        public IEnumerable<T> EnumerateObjects<T>()
            where T : GameObject
        {
            var current = Objects.First;
            while (current != null)
            {
                if (current.Value is T)
                    yield return (T)current.Value;
                current = current.Next;
            }
        }

        /// <summary>
        /// Adds a client to this scene's subscription list
        /// </summary>
        internal void AddSubscription(CogClient client)
        {
            subscribedClients.Add(client);
        }

        public IEnumerable<CogClient> EnumerateSubscribedClients()
        {
            if (!Engine.IsServer)
                throw new Exception("Only the server may enumerate subscribed clients!");
            return subscribedClients;
        }
    }
}
