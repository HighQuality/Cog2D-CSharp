using Cog.Modules;
using Cog.Modules.Content;
using Cog.Modules.EventHost;
using Cog.Modules.Renderer;
using Cog.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Scenes
{
    public class Scene
    {
        public string Name { get; private set; }
        public LinkedList<GameObject> Objects = new LinkedList<GameObject>();
        public Dictionary<long, GameObject> ObjectDictionary = new Dictionary<long, GameObject>();
        public EventModule EventModule = new EventModule();
        private Dictionary<EventIdentifier, List<IEventListener>> eventStrength = new Dictionary<EventIdentifier, List<IEventListener>>();
        private Dictionary<EventIdentifier, IEventListener> globalListeners = new Dictionary<EventIdentifier, IEventListener>();
        private List<EventIdentifier> toBeRemoved = new List<EventIdentifier>();
        private float eventStrengthUpdateTimer = 0f;
        public GameInterface Interface;
        public BaseObject BaseObject { get; private set; }

        public Scene(string name)
        {
            // Randomize an offset for the event strength update timer
            eventStrengthUpdateTimer = Engine.RandomFloat();

            Interface = new GameInterface();
            Interface.Size = Engine.Resolution;

            AddEventStrength<UpdateEvent>(EventModule.RegisterEvent<UpdateEvent>(0, e => { if (Engine.SceneHost.CurrentScene == this) { Update(e); Interface.TriggerUpdate(e); } }));
            AddEventStrength<DrawEvent>(EventModule.RegisterEvent<DrawEvent>(0, e => { if (Engine.SceneHost.CurrentScene == this) Draw(e); }));
            AddEventStrength<DrawInterfaceEvent>(EventModule.RegisterEvent<DrawInterfaceEvent>(0, e => { if (Engine.SceneHost.CurrentScene == this) Interface.TriggerDraw(e, new Vector2()); }));

            // Create an object of this type without invoking the constructor
            BaseObject = (BaseObject)FormatterServices.GetUninitializedObject(typeof(BaseObject));
            BaseObject.Scene = this;
            BaseObject.Id = 0;
            // Invoke the constructor, new()-constraint ensures an empty one exists
            typeof(BaseObject).GetConstructor(new Type[0]).Invoke(BaseObject, new object[0]);

            //BaseObject.LocalCoord = new Vector2(320f, 240f);

            ObjectDictionary.Add(BaseObject.Id, BaseObject);
            Objects.AddLast(BaseObject);
        }

        private void Update(UpdateEvent args)
        {
            BaseObject.LocalRotation -= Angle.FromDegree(args.DeltaTime * 30f);

            var current = Objects.First;
            
            while (current != null)
            {
                if (current.Value.DoRemove)
                    Objects.Remove(current);
                current = current.Next;
            }

            eventStrengthUpdateTimer += args.DeltaTime;
            while(eventStrengthUpdateTimer >= 1f)
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
                    Console.WriteLine("Stopped listening for " + globalListeners[toBeRemoved[i]].IEvent.GetType().GenericTypeArguments[0].Name + " events");

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

        public void Draw(DrawEvent ev)
        {
            DrawTransformation transform = new DrawTransformation();
            transform.WorldScale = Vector2.One;
            transform.ParentWorldScale = Vector2.One;
            BaseObject.Draw(ev, transform);
        }

        public T CreateObject<T>(GameObject parent, Vector2 localCord)
            where T : GameObject, new()
        {
            return CreateObject<T>(null, parent, localCord);
        }

        public T CreateObject<T>(CogClient owner, GameObject parent, Vector2 localCoord)
            where T : GameObject, new()
        {
            if (Engine.IsNetworkGame && !Engine.IsServer)
                throw new Exception("Only the server can create global objects!");
            if (parent == null)
                throw new Exception("\"parent\" may not be null!, use Scene.BaseObject instead");
            
            // Create an object of this type without invoking the constructor
            T obj = (T)FormatterServices.GetUninitializedObject(typeof(T));
            obj.Scene = this;
            obj.Owner = owner;
            obj.Id = Engine.GetGlobalId();
            obj.Parent = parent;
            obj.LocalCoord = localCoord;
            // Invoke the constructor, new()-constraint ensures an empty one exists
            typeof(T).GetConstructor(new Type[0]).Invoke(obj, new object[0]);
            
            ObjectDictionary.Add(obj.Id, obj);
            Objects.AddLast(obj);

            // Serialize and send to clients that are subscribed to this scene
            if (Engine.IsServer)
            {
                CreateObjectMessage message = obj.CreateCreationMessage();

                Engine.ServerModule.Send<CreateObjectMessage>(message);
            }

            return obj;
        }

        public T CreateLocalObject<T>(Vector2 position)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            var current = Objects.First;
            do
            {
                if (current.Value.DoRemove)
                    current.Value.Remove();
            }
            while (current != null);

            Objects.Clear();
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
    }
}
