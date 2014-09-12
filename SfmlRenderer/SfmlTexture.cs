using Square.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square.SfmlRenderer
{
    public class SfmlTexture : ITexture
    {
        internal SFML.Graphics.Texture Texture;
        public Vector2 Size { get { return new Vector2(Texture.Size.X, Texture.Size.Y); } }

        internal SfmlTexture(string filename)
        {
            this.Texture = new SFML.Graphics.Texture(filename);
        }

        internal SfmlTexture(Image image)
        {
            var sfmlImage = new SFML.Graphics.Image((uint)image.Width, (uint)image.Height);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var color = image.GetColor(x, y);
                    sfmlImage.SetPixel((uint)x, (uint)y, new SFML.Graphics.Color((byte)color.R, (byte)color.G, (byte)color.B, (byte)color.A));
                }
            }
            this.Texture = new SFML.Graphics.Texture(sfmlImage);
        }
    }
}
