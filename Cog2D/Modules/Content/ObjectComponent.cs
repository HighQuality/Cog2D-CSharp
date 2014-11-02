﻿using Cog.Modules.EventHost;
using Cog.Modules.Renderer;
using Cog.Scenes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public abstract class ObjectComponent
    {
        internal static Dictionary<Type, Action<EventModule, ObjectComponent>> RegistratorCache;
        internal static Dictionary<Type, ComponentSerializer> SerializerCache;
        internal static List<ComponentSerializer> Serializers;
        internal static Dictionary<FieldInfo, SynchronizedEditPermission[]> SynchronizedPermissions;
        internal static UInt16 NextComponentId;

        private List<IEventListener> registeredFunctions; public GameObject GameObject { get; internal set; }
        public Scene Scene { get { return GameObject.Scene; } }
        public Vector2 LocalCoord { get { return GameObject.LocalCoord; } set { GameObject.LocalCoord = value; } }
        public Vector2 WorldCoord { get { return GameObject.WorldCoord; } set { GameObject.WorldCoord = value; } }

        public ObjectComponent()
        {
        }

        [ComponentEvent(true)]
        public virtual bool KeyDown(Keyboard.Key key) { return false; }
        public virtual void KeyUp(Keyboard.Key key) { }
        [ComponentEvent(true)]
        public virtual void Update(UpdateEvent ev) { }
        [ComponentEvent(true)]
        public virtual void PhysicsUpdate(PhysicsUpdateEvent ev) { }
        [ComponentEvent(true)]
        public virtual void Draw(DrawEvent ev, DrawTransformation transformation) { }
        [ComponentEvent(true)]
        public virtual void DrawInterface(IRenderTarget renderTarget) { }

        public virtual void ObjectRemoved() { }
        public virtual void ComponentRemoved() { }

        internal void DeregisterFunctions()
        {
            // Cancel registered event listeners
            for (int i = registeredFunctions.Count - 1; i >= 0; i--)
                registeredFunctions[i].Cancel();
            registeredFunctions.Clear();
        }
        
        internal static void InitializeCache()
        {
            SerializerCache = new Dictionary<Type, ComponentSerializer>();
            Serializers = new List<ComponentSerializer>();
            SynchronizedPermissions = new Dictionary<FieldInfo, SynchronizedEditPermission[]>();
            NextComponentId = 1;
        }

        internal static ComponentSerializer CreateSerializer(Type type)
        {
            if (SerializerCache.ContainsKey(type))
                throw new InvalidOperationException("Cache for object component \"" + type.FullName + "\" already exists!");

            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(o => typeof(ISynchronized).IsAssignableFrom(o.FieldType)).OrderBy(o => o.Name).ToArray();

            Action<ObjectComponent, BinaryWriter>[] writers = new Action<ObjectComponent, BinaryWriter>[fields.Length];
            Action<ObjectComponent, BinaryReader>[] readers = new Action<ObjectComponent, BinaryReader>[fields.Length];
            SynchronizedEditPermission[] permissionCollection = new SynchronizedEditPermission[fields.Length];

            for (int i=0; i<fields.Length; i++)
            {
                var field = fields[i];

                var editAttribute = field.GetCustomAttribute<ClientEditAttribute>();
                SynchronizedEditPermission permissions;
                if (editAttribute != null)
                {
                    permissions.CanEdit = editAttribute.CanEdit;
                    permissions.RequireOwner = editAttribute.RequireOwner;
                }
                else
                {
                    permissions.CanEdit = false;
                    permissions.RequireOwner = false;
                }
                permissionCollection[i] = permissions;
                
                var innerType = field.FieldType.GenericTypeArguments[0];
                var typeSerializer = TypeSerializer.GetTypeWriter(innerType);
                if (typeSerializer != null)
                {
                    writers[i] = (c, w) => typeSerializer.GenericWrite(field.GetValue(c), w);
                    readers[i] = (c, r) => field.SetValue(c, typeSerializer.GenericRead(r));
                }
                else
                    throw new NoSerializerException(string.Format("Synchronized<{0}> {1}.{2} doesn't have a type serializer!", innerType.FullName, type.FullName, field.Name));
            }
            //TODO: Merge individual writers and readers to same variable as global, iterate through them all for global read / write, move them from seperate dictionary into this one (same with permissions)
            var s = new ComponentSerializer(type, writers, readers, NextComponentId++);
            SerializerCache.Add(type, s);
            Serializers.Add(s);
            return s;
        }

        internal static Action<EventModule, ObjectComponent> CreateEventRegistrator(Type type)
        {
            Action<EventModule, ObjectComponent> registrator = null;
            HashSet<string> alreadyRegistered = new HashSet<string>();

            // Iterate all of this component's methods to figure out which ones to register as events
            foreach (var method in type.GetMethods())
            {
                //Require a ComponentEventAttribute to register the method
                var eventAttribute = (ComponentEventAttribute)method.GetCustomAttributes(typeof(ComponentEventAttribute), true).FirstOrDefault();
                if (eventAttribute == null)
                    continue;
                // If it requires an overload to register, enforce it
                if (eventAttribute.RequireOverride)
                {
                    var baseDef = method.GetBaseDefinition();
                    if (baseDef == null || baseDef == method || baseDef.DeclaringType == method.DeclaringType)
                        continue;
                }

                // Check if it's already registered (Multiple overrides) / create a dictionary for registered functions
                if (alreadyRegistered.Contains(method.Name))
                    continue;
                alreadyRegistered.Add(method.Name);

                // All tests were passed, add the method regitrator
                string func = method.Name;
                if (func == "Update")
                    registrator += (ev, comp) => comp.RegisterEvent<UpdateEvent>(0, e => comp.Update(e));
                else if (func == "PhysicsUpdate")
                    registrator += (ev, comp) => comp.RegisterEvent<PhysicsUpdateEvent>(0, e => comp.PhysicsUpdate(e));
                else if (func == "Draw")
                    registrator += (ev, comp) => { comp.GameObject.OnDraw += (e, t) => { comp.Draw(e, t); }; };
                else if (func == "KeyDown")
                    registrator += (ev, comp) => comp.RegisterEvent<KeyDownEvent>(0, e => { if (comp.KeyDown(e.Key)) { e.KeyUpEvent = () => comp.KeyUp(e.Key); e.Intercept = true; } });
                else if (func == "DrawInterface")
                    registrator += (ev, comp) => comp.RegisterEvent<DrawInterfaceEvent>(0, e => comp.DrawInterface(e.RenderTarget));
                else
                    Console.WriteLine("Tried to register function with no registration handler: " + func);
                }

            // Cache the registerer
            RegistratorCache.Add(type, registrator);

            return registrator;
        }
        
        internal static ComponentSerializer GetSerializer(Type type)
        {
            return SerializerCache[type];
        }

        internal static ComponentSerializer GetSerializer(UInt16 id)
        {
            return Serializers[id];
        }

        internal void RegisterFunctions(EventModule events)
        {
            Action<EventModule, ObjectComponent> registrator = null;
            bool didRegister = false;
            if (RegistratorCache != null)
            {
                // Try to get a cached event registerer
                if (RegistratorCache.TryGetValue(GetType(), out registrator))
                {
                    // Components without events are still cached
                    if (registrator != null)
                    {
                        registrator(events, this);
                    }
                    didRegister = true;
                }
            }

            if (!didRegister)
            {
                registrator = CreateEventRegistrator(GetType());

                // Register our events
                if (registrator != null)
                {
                    registrator(events, this);
                }
            }
        }
        
        public EventListener<T> RegisterEvent<T>(int priority, Action<T> action)
            where T : EventParameters
        {
            return RegisterEvent<T>(EventModule.DefaultUniqueIdentifier, priority, action);
        }

        public EventListener<T> RegisterEvent<T>(object uniqueIdentifier, int priority, Action<T> action)
            where T : EventParameters
        {
            var listener = Scene.EventModule.RegisterEvent(uniqueIdentifier, priority, action);
            Scene.AddEventStrength<T>(listener);
            if (registeredFunctions == null)
                registeredFunctions = new List<IEventListener>();
            registeredFunctions.Add(listener);
            return listener;
        }

        public KeyCapture CaptureKey(Keyboard.Key key, int priority, CaptureRelayMode relayMode)
        {
            if (relayMode != CaptureRelayMode.NoRelay && !Engine.IsNetworkGame)
                throw new ArgumentOutOfRangeException("Only CaptureRelayMode.NoRelay may be used in non-multiplayer games!");
            return new KeyCapture(GameObject, key, priority, relayMode);
        }
    }
}
