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
    }
}
