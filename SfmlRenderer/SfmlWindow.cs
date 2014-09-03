using SFML.Graphics;
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

        private string _title;
        public string Title { get { return _title; } set { _title = value; InnerWindow.SetTitle(_title); } }
        public Vector2 Size { get { var size = InnerWindow.Size; return new Vector2((float)size.X, (float)size.Y); } set { InnerWindow.Size = new SFML.System.Vector2u((uint)value.X, (uint)value.Y); } }
        public Vector2 Position { get { var position = InnerWindow.Position; return new Vector2((float)position.X, (float)position.Y); } set { InnerWindow.Position = new SFML.System.Vector2i((int)value.X, (int)value.Y); } }
        private bool _visible;
        public bool Visible { get { return _visible; } set { _visible = value; InnerWindow.SetVisible(value); } }
        public bool IsOpen { get { return InnerWindow.IsOpen; } }
        bool _vsync;
        public bool VerticalSynchronization { get { return _vsync; } set { _vsync = value; InnerWindow.SetVerticalSyncEnabled(_vsync); } }

        internal IEventHostModule Events;

        public IRenderTarget RenderTarget { get { return this; } }

        public SfmlWindow(string title, int width, int height, WindowStyle style)
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

            InnerWindow.KeyPressed += InnerWindow_KeyPressed;
            InnerWindow.KeyReleased += InnerWindow_KeyReleased;
        }

        void InnerWindow_KeyPressed(object sender, SFML.Window.KeyEventArgs e)
        {
        }
        void InnerWindow_KeyReleased(object sender, SFML.Window.KeyEventArgs e)
        {
        }

        public void DispatchEvents()
        {
            InnerWindow.DispatchEvents();
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

        public void CreateInputEvents(IEventHostModule eventHost)
        {
            this.Events = eventHost;

            for (int i = 0; i < (int)SFML.Window.Keyboard.Key.KeyCount; i++)
            {
                var key = ((SFML.Window.Keyboard.Key)i).ToString();
                eventHost.CreateEvent(key + " up");
                eventHost.CreateEvent(key + " pressed");
            }
        }

        public void RenderTexture(ITexture texture, Vector2 worldCoords)
        {
            Sprite sprite = new Sprite();
            sprite.Position = new SFML.System.Vector2f(worldCoords.X, worldCoords.Y);
            sprite.Texture = ((SfmlTexture)texture).Texture;
            sprite.Draw(InnerWindow, new RenderStates());
        }
    }
}
