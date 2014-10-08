using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cog;
using Cog.SfmlRenderer;
using Cog.Modules.Renderer;
using Cog.Modules.EventHost;
using Cog.Modules.Content;
using System.Threading;
using Cog.Modules.Networking;

namespace TestGame
{
    class Program
    {
        static ITexture window;

        static void DrawWindow(Rectangle rect, IRenderTarget target)
        {
            // Top Left
            target.RenderTexture(window, rect.TopLeft, Vector2.One, Vector2.Zero, 0f, new Rectangle(0f, 0f, 8f, 20f));
            // Top Right
            target.RenderTexture(window, rect.TopRight - new Vector2(8f, 0f), Vector2.One, Vector2.Zero, 0f, new Rectangle(window.Size.X - 8f, 0f, 8f, 20f));
            // Bottom Left
            target.RenderTexture(window, rect.BottomLeft - new Vector2(0f, 8f), Vector2.One, Vector2.Zero, 0f, new Rectangle(0f, 23f, 8f, 8f));
            // Bottom Right
            target.RenderTexture(window, rect.BottomRight - new Vector2(8f, 8f), Vector2.One, Vector2.Zero, 0f, new Rectangle(11f, 23f, 8f, 8f));

            // Top
            target.RenderTexture(window, rect.TopLeft + new Vector2(8f, 0f), new Vector2(rect.Size.X - 16f, 1f), Vector2.Zero, 0f, new Rectangle(9f, 0f, 1f, 20f));
            // Bottom
            target.RenderTexture(window, rect.BottomLeft + new Vector2(8f, -8f), new Vector2(rect.Size.X - 16f, 1f), Vector2.Zero, 0f, new Rectangle(9f, 23, 1f, 8f));
            // Left
            target.RenderTexture(window, rect.TopLeft + new Vector2(0f, 20f), new Vector2(1f, rect.Size.Y - 28f), Vector2.Zero, 0f, new Rectangle(0f, 21f, 8f, 1f));
            // Right
            target.RenderTexture(window, rect.TopRight + new Vector2(-8f, 20f), new Vector2(1f, rect.Size.Y - 28f), Vector2.Zero, 0f, new Rectangle(11f, 21f, 8f, 1f));

            // Middle Texture
            target.RenderTexture(window, rect.TopLeft + new Vector2(8f, 20f), rect.Size - new Vector2(16f, 28f), Vector2.Zero, 0f, new Rectangle(9f, 21f, 1f, 1f));

        }

        static void Main(string[] args)
        {
            Engine.Initialize<SfmlRenderer>(new Cog.Image("splash.png"));
            float time = 0f;
            
            Engine.EventHost.RegisterEvent<InitializeEvent>(0, e =>
            {
                /*var message = Engine.ConnectServer("127.0.0.1", 1234);
                if (message != null)
                    Debug.Error(message);
                else
                    Debug.Success("Successfully connected to server @{0}:{1}!", Engine.ClientModule.Hostname, Engine.ClientModule.Port);*/

                window = Engine.Renderer.LoadTexture("blue_window.png");

                // Create and push the initial scene
                Engine.SceneHost.Push(new GameScene());
            });
            
            Engine.EventHost.RegisterEvent<UpdateEvent>(1, e =>
            {
                time += e.DeltaTime;
            });
            
            Engine.EventHost.RegisterEvent<DrawEvent>(1, e =>
            {

            });

            Engine.EventHost.RegisterEvent<ExitEvent>(0, e =>
            {
            });

            Engine.StartGame("Test Game", 640, 480, WindowStyle.Default);
        }
    }
}
