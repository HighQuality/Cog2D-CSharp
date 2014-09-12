using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Square;
using Square.SfmlRenderer;
using Square.Modules.Renderer;
using Square.Modules.EventHost;
using Square.Modules.Content;
using System.Diagnostics;

namespace TestGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine.Initialize<SfmlRenderer>();

            ITexture texture = null;

            Engine.EventHost.RegisterEvent<KeyDownEvent>(Keyboard.Key.Space, 1, e =>
            {
                Image image = new Image(640, 480);
                float zoom = 250f;
                float xOffset = Engine.RandomFloat() * 100f,
                    yOffset = Engine.RandomFloat() * 100f;

                Stopwatch watch = Stopwatch.StartNew();
                for (int y=0; y<image.Height; y++)
                {
                    for (int x=0; x<image.Width; x++)
                    {
                        float distance = (float)Math.Sqrt(Math.Pow(640f / 2f - x, 2f) + Math.Pow(480f / 2f - y, 2f));

                        float val = (Noise.Generate((float)x / zoom + xOffset, (float)y / zoom + yOffset) + 1f) / 2f;
                        //val *= (Noise.Generate((float)x / zoom + 100f + xOffset, (float)y / zoom + 100f + yOffset) + 1f) / 2f;
                        val *= 1.5f - (distance / 200f);
                        Color color = Color.White;

                        if (val < 0.25f)
                            color = Color.Blue;
                        else if (val < 0.8f)
                            color = Color.Green;
                        else
                            color = Color.Gray;

                        image.SetColor(x, y, color);
                    }
                }

                texture = Engine.Renderer.TextureFromImage(image);
                Console.WriteLine(watch.Elapsed.TotalMilliseconds);
            });

            Engine.EventHost.RegisterEvent<UpdateEvent>(1, e =>
            {

            });

            Engine.EventHost.RegisterEvent<DrawEvent>(1, e =>
            {
                if (texture != null)
                    e.RenderTarget.RenderTexture(texture, new Vector2());
            });
                        
            Engine.StartGame("Test Game", 640, 480, WindowStyle.Default);
        }
    }
}
