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
    public class SfmlWindow : Window, Modules.Renderer.IRenderTarget
    {
        internal RenderWindow InnerWindow;

        private Dictionary<SFML.Window.Keyboard.Key, Action> keyUpEvents = new Dictionary<SFML.Window.Keyboard.Key, Action>();
        private LinkedList<SFML.Window.Keyboard.Key> pressedKeys = new LinkedList<SFML.Window.Keyboard.Key>();
        bool _vsync;
        public override bool VerticalSynchronization { get { return _vsync; } set { _vsync = value; InnerWindow.SetVerticalSyncEnabled(_vsync); } }

        public override IRenderTarget RenderTarget { get { return this; } }

        private bool[] mouseButtons = new bool[(int)SFML.Window.Mouse.Button.ButtonCount];

        public SfmlWindow(string title, int width, int height, WindowStyle style)
            : base(title, width, height, style)
        {
            var contextSettings = new SFML.Window.ContextSettings(0, 0, 16);

            InnerWindow = new RenderWindow(Handle, contextSettings);
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

        public override void Clear(Color color)
        {
            InnerWindow.Clear(new SFML.Graphics.Color((byte)color.R, (byte)color.G, (byte)color.B, (byte)color.A));
        }

        public override void Display()
        {
            InnerWindow.Display();
        }
        
        public void DrawTexture(Modules.Renderer.Texture texture, Vector2 windowCoords)
        {
            Sprite sprite = new Sprite();
            sprite.Position = new SFML.System.Vector2f(windowCoords.X, windowCoords.Y);
            sprite.Texture = ((SfmlTexture)texture).Texture;
            sprite.Scale = new SFML.System.Vector2f(1f, 1f);
            InnerWindow.Draw(sprite, SfmlRenderer.RenderState);
        }

        public void DrawTexture(Modules.Renderer.Texture texture, Vector2 windowCoords, Color color, Vector2 scale, Vector2 origin, float rotation, Rectangle textureRect)
        {
            Sprite sprite = new Sprite();
            sprite.Position = new SFML.System.Vector2f(windowCoords.X, windowCoords.Y);
            sprite.Color = new SFML.Graphics.Color((byte)color.R, (byte)color.G, (byte)color.B, (byte)color.A);
            sprite.Origin = new SFML.System.Vector2f(origin.X, origin.Y);
            sprite.Scale = new SFML.System.Vector2f(scale.X, scale.Y);
            sprite.Texture = ((SfmlTexture)texture).Texture;
            sprite.Rotation = rotation;
            sprite.TextureRect = new IntRect((int)textureRect.TopLeft.X, (int)textureRect.TopLeft.Y, (int)textureRect.Size.X, (int)textureRect.Size.Y);
            InnerWindow.Draw(sprite, SfmlRenderer.RenderState);
        }
    }
}
