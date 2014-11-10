using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.SfmlRenderer
{
    public class SfmlTexture : Texture
    {
        internal SFML.Graphics.Texture Texture;
        public override Vector2 Size { get { return new Vector2(Texture.Size.X, Texture.Size.Y); } }

        internal SfmlTexture(string filename)
        {
            this.Texture = new SFML.Graphics.Texture(filename);
            IsLoaded = true;
        }

        internal SfmlTexture(byte[] data)
        {
            this.Texture = new SFML.Graphics.Texture(data);
            IsLoaded = true;
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

            IsLoaded = true;
            IsDynamic = true;
        }

        private void Dispose(bool disposed)
        {
            if (disposed)
            {
                Texture.Dispose();
                Texture = null;
                IsLoaded = false;
            }
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SfmlTexture()
        {
            Dispose(false);
        }
    }
}
