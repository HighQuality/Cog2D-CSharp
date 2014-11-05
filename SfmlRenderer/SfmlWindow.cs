using SFML.Graphics;
using Cog.Modules.EventHost;
using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.SfmlRenderer
{
    public class SfmlWindow : IWindow, IRenderTarget
    {
        internal RenderWindow InnerWindow;

        private Dictionary<SFML.Window.Keyboard.Key, Action> keyUpEvents = new Dictionary<SFML.Window.Keyboard.Key, Action>();
        public LinkedList<SFML.Window.Keyboard.Key> pressedKeys = new LinkedList<SFML.Window.Keyboard.Key>();
        private string _title;
        public string Title { get { return _title; } set { _title = value; InnerWindow.SetTitle(_title); } }
        public Vector2 Resolution { get { var size = InnerWindow.Size; return new Vector2((float)size.X, (float)size.Y); } set { InnerWindow.Size = new SFML.System.Vector2u((uint)value.X, (uint)value.Y); } }
        public Vector2 Position { get { var position = InnerWindow.Position; return new Vector2((float)position.X, (float)position.Y); } set { InnerWindow.Position = new SFML.System.Vector2i((int)value.X, (int)value.Y); } }
        private bool _visible;
        public bool Visible { get { return _visible; } set { _visible = value; InnerWindow.SetVisible(value); } }
        public bool IsOpen { get { return InnerWindow.IsOpen; } }
        bool _vsync;
        public bool VerticalSynchronization { get { return _vsync; } set { _vsync = value; InnerWindow.SetVerticalSyncEnabled(_vsync); } }
        
        public IRenderTarget RenderTarget { get { return this; } }

        public EventModule EventHost;
        private bool[] mouseButtons = new bool[(int)SFML.Window.Mouse.Button.ButtonCount];

        public SfmlWindow(string title, int width, int height, WindowStyle style, EventModule eventHost)
        {
            if (reverseKeyMap == null)
            {
                reverseKeyMap = new Dictionary<Keyboard.Key,SFML.Window.Keyboard.Key>();

                foreach (var pair in keymap)
                {
                    reverseKeyMap[pair.Value] = pair.Key;
                }
            }

            SFML.Window.Styles sfmlStyle;
            switch(style)
            {
                case WindowStyle.Default:
                    sfmlStyle = SFML.Window.Styles.Close;
                    break;
                case WindowStyle.Fullscreen:
                    sfmlStyle = SFML.Window.Styles.Fullscreen;
                    break;
                case WindowStyle.Resizable:
                    sfmlStyle = SFML.Window.Styles.Resize;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("style");
            }

            InnerWindow = new RenderWindow(new SFML.Window.VideoMode((uint)width, (uint)height), title, sfmlStyle);
            _title = title;
            _visible = true;

            this.EventHost = eventHost;

            InnerWindow.KeyPressed += InnerWindow_KeyPressed;
            InnerWindow.MouseButtonPressed += InnerWindow_MouseButtonPressed;
            InnerWindow.MouseMoved += InnerWindow_MouseMoved;
            InnerWindow.Closed += InnerWindow_Closed;
        }

        private void InnerWindow_MouseMoved(object sender, SFML.Window.MouseMoveEventArgs e)
        {
            Mouse.Location = new Vector2(e.X, e.Y);
        }

        private void InnerWindow_MouseButtonPressed(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            if ((int)e.Button >= sfmlButtonToCog.Length)
                return;
            mouseButtons[(int)e.Button] = true;
            Mouse.SetDown(sfmlButtonToCog[(int)e.Button]);
        }

        void InnerWindow_Closed(object sender, EventArgs e)
        {
            Engine.EventHost.GetEvent<CloseButtonEvent>().Trigger(new CloseButtonEvent(this));
        }

        private static Mouse.Button[] sfmlButtonToCog = new Mouse.Button[3] { Mouse.Button.Left, Mouse.Button.Right, Mouse.Button.Middle };
        private static SFML.Window.Mouse.Button[] cogToSfml = new SFML.Window.Mouse.Button[3] { SFML.Window.Mouse.Button.Left, SFML.Window.Mouse.Button.Middle, SFML.Window.Mouse.Button.Right };

        private static Dictionary<SFML.Window.Keyboard.Key, Keyboard.Key> keymap = new Dictionary<SFML.Window.Keyboard.Key, Keyboard.Key>
        {
            { SFML.Window.Keyboard.Key.Space, Keyboard.Key.Space },
            { SFML.Window.Keyboard.Key.Left, Keyboard.Key.Left },
            { SFML.Window.Keyboard.Key.Right, Keyboard.Key.Right },
            { SFML.Window.Keyboard.Key.Up, Keyboard.Key.Up },
            { SFML.Window.Keyboard.Key.Down, Keyboard.Key.Down },
            { SFML.Window.Keyboard.Key.LShift, Keyboard.Key.LShift },
            { SFML.Window.Keyboard.Key.RShift, Keyboard.Key.RShift },
            { SFML.Window.Keyboard.Key.LControl, Keyboard.Key.LCtrl },
            { SFML.Window.Keyboard.Key.RControl, Keyboard.Key.RCtrl },
            { SFML.Window.Keyboard.Key.LAlt, Keyboard.Key.LAlt },
            { SFML.Window.Keyboard.Key.RAlt, Keyboard.Key.RAlt },
            { SFML.Window.Keyboard.Key.Escape, Keyboard.Key.Escape },
            { SFML.Window.Keyboard.Key.Tab, Keyboard.Key.Tab },

            { SFML.Window.Keyboard.Key.A, Keyboard.Key.A },
            { SFML.Window.Keyboard.Key.B, Keyboard.Key.B },
            { SFML.Window.Keyboard.Key.C, Keyboard.Key.C },
            { SFML.Window.Keyboard.Key.D, Keyboard.Key.D },
            { SFML.Window.Keyboard.Key.E, Keyboard.Key.E },
            { SFML.Window.Keyboard.Key.F, Keyboard.Key.F },
            { SFML.Window.Keyboard.Key.G, Keyboard.Key.G },
            { SFML.Window.Keyboard.Key.H, Keyboard.Key.H },
            { SFML.Window.Keyboard.Key.I, Keyboard.Key.I },
            { SFML.Window.Keyboard.Key.J, Keyboard.Key.J },
            { SFML.Window.Keyboard.Key.K, Keyboard.Key.K },
            { SFML.Window.Keyboard.Key.L, Keyboard.Key.L },
            { SFML.Window.Keyboard.Key.M, Keyboard.Key.M },
            { SFML.Window.Keyboard.Key.N, Keyboard.Key.N },
            { SFML.Window.Keyboard.Key.O, Keyboard.Key.O },
            { SFML.Window.Keyboard.Key.P, Keyboard.Key.P },
            { SFML.Window.Keyboard.Key.Q, Keyboard.Key.Q },
            { SFML.Window.Keyboard.Key.R, Keyboard.Key.R },
            { SFML.Window.Keyboard.Key.S, Keyboard.Key.S },
            { SFML.Window.Keyboard.Key.T, Keyboard.Key.T },
            { SFML.Window.Keyboard.Key.U, Keyboard.Key.U },
            { SFML.Window.Keyboard.Key.V, Keyboard.Key.V },
            { SFML.Window.Keyboard.Key.W, Keyboard.Key.W },
            { SFML.Window.Keyboard.Key.X, Keyboard.Key.X },
            { SFML.Window.Keyboard.Key.Y, Keyboard.Key.Y },
            { SFML.Window.Keyboard.Key.Z, Keyboard.Key.Z },
            
            { SFML.Window.Keyboard.Key.F1, Keyboard.Key.F1 },
            { SFML.Window.Keyboard.Key.F2, Keyboard.Key.F2 },
            { SFML.Window.Keyboard.Key.F3, Keyboard.Key.F3 },
            { SFML.Window.Keyboard.Key.F4, Keyboard.Key.F4 },
            { SFML.Window.Keyboard.Key.F5, Keyboard.Key.F5 },
            { SFML.Window.Keyboard.Key.F6, Keyboard.Key.F6 },
            { SFML.Window.Keyboard.Key.F7, Keyboard.Key.F7 },
            { SFML.Window.Keyboard.Key.F8, Keyboard.Key.F8 },
            { SFML.Window.Keyboard.Key.F9, Keyboard.Key.F9 },
            { SFML.Window.Keyboard.Key.F10, Keyboard.Key.F10 },
            { SFML.Window.Keyboard.Key.F11, Keyboard.Key.F11 },
            { SFML.Window.Keyboard.Key.F12, Keyboard.Key.F12 },
            
            { SFML.Window.Keyboard.Key.Num0, Keyboard.Key.Num0 },
            { SFML.Window.Keyboard.Key.Num1, Keyboard.Key.Num1 },
            { SFML.Window.Keyboard.Key.Num2, Keyboard.Key.Num2 },
            { SFML.Window.Keyboard.Key.Num3, Keyboard.Key.Num3 },
            { SFML.Window.Keyboard.Key.Num4, Keyboard.Key.Num4 },
            { SFML.Window.Keyboard.Key.Num5, Keyboard.Key.Num5 },
            { SFML.Window.Keyboard.Key.Num6, Keyboard.Key.Num6 },
            { SFML.Window.Keyboard.Key.Num7, Keyboard.Key.Num7 },
            { SFML.Window.Keyboard.Key.Num8, Keyboard.Key.Num8 },
            { SFML.Window.Keyboard.Key.Num9, Keyboard.Key.Num9 },
        };

        private static Dictionary<Keyboard.Key, SFML.Window.Keyboard.Key> reverseKeyMap;

        private Keyboard.Key SfmlKeyToCog(SFML.Window.Keyboard.Key key)
        {
            Keyboard.Key cogKey;
            if (keymap.TryGetValue(key, out cogKey))
                return cogKey;
            return Keyboard.Key.Unknown;
        }

        private SFML.Window.Keyboard.Key CogKeyToSfml(Keyboard.Key key)
        {
            SFML.Window.Keyboard.Key sfmlKey;
            if (reverseKeyMap.TryGetValue(key, out sfmlKey))
                return sfmlKey;
            return SFML.Window.Keyboard.Key.Unknown;
        }

        void InnerWindow_KeyPressed(object sender, SFML.Window.KeyEventArgs e)
        {
            if (e.Code == SFML.Window.Keyboard.Key.Unknown)
                return;

            if (!keyUpEvents.ContainsKey(e.Code))
            {
                var newKey = SfmlKeyToCog(e.Code);
                if (newKey == Keyboard.Key.Unknown)
                {
                    Console.WriteLine(e.Code.ToString() + " is not mapped to a Cog.Keyboard.Key");
                    return;
                }

                var keyParameters = new KeyDownEvent(this, newKey);
                // If a specific key handler didn't intercept the event, send it on to the generic key handler event
                if (!EventHost.GetEvent<KeyDownEvent>(newKey).Trigger(keyParameters))
                    EventHost.GetEvent<KeyDownEvent>().Trigger(keyParameters);
                keyUpEvents.Add(e.Code, keyParameters.KeyUpEvent);
                pressedKeys.AddLast(e.Code);
            }
        }

        public void DispatchEvents()
        {
            InnerWindow.DispatchEvents();

            var node = pressedKeys.First;
            while (node != null)
            {
                if (!SFML.Window.Keyboard.IsKeyPressed(node.Value))
                {
                    var action = keyUpEvents[node.Value];
                    if (action != null)
                        action();
                    keyUpEvents.Remove(node.Value);
                    pressedKeys.Remove(node);
                }
                node = node.Next;
            }
            
            for (int i=0; i<mouseButtons.Length; i++)
            {
                if (i >= sfmlButtonToCog.Length)
                    continue;

                if (mouseButtons[i] && !SFML.Window.Mouse.IsButtonPressed((SFML.Window.Mouse.Button)i))
                {
                    Mouse.SetReleased(sfmlButtonToCog[i]);
                    mouseButtons[i] = false;
                }
            }
        }
        
        public void SetTransformation(Vector2 center, Vector2 scale, Angle rotation)
        {
            var size = new SFML.System.Vector2f();
            size.X = (Resolution.X / scale.X);
            size.Y = (Resolution.Y / scale.Y);
            var view = new View(new SFML.System.Vector2f(center.X, center.Y), size);
            view.Rotation = rotation.Degree;

            InnerWindow.SetView(view);
        }

        public void Clear(Color color)
        {
            InnerWindow.Clear(new SFML.Graphics.Color((byte)color.R, (byte)color.G, (byte)color.B, (byte)color.A));
        }

        public void Display()
        {
            InnerWindow.Display();
        }

        public void Close()
        {
            InnerWindow.Close();
        }

        public void ApplyChanges()
        {
        }
        
        public void RenderTexture(ITexture texture, Vector2 windowCoords)
        {
            Sprite sprite = new Sprite();
            sprite.Position = new SFML.System.Vector2f(windowCoords.X, windowCoords.Y);
            sprite.Texture = ((SfmlTexture)texture).Texture;
            sprite.Scale = new SFML.System.Vector2f(1f, 1f);
            InnerWindow.Draw(sprite);
        }

        public void RenderTexture(ITexture texture, Vector2 windowCoords, Color color, Vector2 scale, Vector2 origin, float rotation, Rectangle textureRect)
        {
            Sprite sprite = new Sprite();
            sprite.Position = new SFML.System.Vector2f(windowCoords.X, windowCoords.Y);
            sprite.Color = new SFML.Graphics.Color((byte)color.R, (byte)color.G, (byte)color.B, (byte)color.A);
            sprite.Origin = new SFML.System.Vector2f(origin.X, origin.Y);
            sprite.Scale = new SFML.System.Vector2f(scale.X, scale.Y);
            sprite.Texture = ((SfmlTexture)texture).Texture;
            sprite.Rotation = rotation;
            sprite.TextureRect = new IntRect((int)textureRect.TopLeft.X, (int)textureRect.TopLeft.Y, (int)textureRect.Size.X, (int)textureRect.Size.Y);
            InnerWindow.Draw(sprite);
        }

        public bool IsKeyDown(Keyboard.Key key)
        {
            return pressedKeys.Contains(CogKeyToSfml(key));
        }
    }
}
