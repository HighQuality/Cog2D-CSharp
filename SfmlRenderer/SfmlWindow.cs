﻿using SFML.Graphics;
using Square.Modules.EventHost;
using Square.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.SfmlRenderer
{
    public class SfmlWindow : IWindow, IRenderTarget
    {
        internal RenderWindow InnerWindow;

        private Dictionary<SFML.Window.Keyboard.Key, Action> keyUpEvents = new Dictionary<SFML.Window.Keyboard.Key, Action>();
        public LinkedList<SFML.Window.Keyboard.Key> pressedKeys = new LinkedList<SFML.Window.Keyboard.Key>();
        private string _title;
        public string Title { get { return _title; } set { _title = value; InnerWindow.SetTitle(_title); } }
        public Vector2 Size { get { var size = InnerWindow.Size; return new Vector2((float)size.X, (float)size.Y); } set { InnerWindow.Size = new SFML.System.Vector2u((uint)value.X, (uint)value.Y); } }
        public Vector2 Position { get { var position = InnerWindow.Position; return new Vector2((float)position.X, (float)position.Y); } set { InnerWindow.Position = new SFML.System.Vector2i((int)value.X, (int)value.Y); } }
        private bool _visible;
        public bool Visible { get { return _visible; } set { _visible = value; InnerWindow.SetVisible(value); } }
        public bool IsOpen { get { return InnerWindow.IsOpen; } }
        bool _vsync;
        public bool VerticalSynchronization { get { return _vsync; } set { _vsync = value; InnerWindow.SetVerticalSyncEnabled(_vsync); } }
        
        public IRenderTarget RenderTarget { get { return this; } }

        public EventModule EventHost;

        public SfmlWindow(string title, int width, int height, WindowStyle style, EventModule eventHost)
        {
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
            InnerWindow.Closed += InnerWindow_Closed;
        }

        void InnerWindow_Closed(object sender, EventArgs e)
        {
            Engine.EventHost.GetEvent<CloseButtonEvent>().Trigger(new CloseButtonEvent(this));
        }

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

        private Keyboard.Key SfmlKeyToSquare(SFML.Window.Keyboard.Key key)
        {
            Keyboard.Key squareKey;
            if (keymap.TryGetValue(key, out squareKey))
                return squareKey;
            return Keyboard.Key.Unknown;
        }

        void InnerWindow_KeyPressed(object sender, SFML.Window.KeyEventArgs e)
        {
            if (e.Code == SFML.Window.Keyboard.Key.Unknown)
                return;

            if (!keyUpEvents.ContainsKey(e.Code))
            {
                var newKey = SfmlKeyToSquare(e.Code);
                if (newKey == Keyboard.Key.Unknown)
                {
                    Console.WriteLine(e.Code.ToString() + " is not mapped to a Square.Keyboard.Key");
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
    }
}
