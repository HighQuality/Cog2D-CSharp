using Cog.Modules.Renderer;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XnaRenderer
{
    public class XnaTexture : Cog.Modules.Renderer.Texture
    {
        internal Texture2D InnerTexture;
        public override Cog.Vector2 Size
        {
            get { return new Cog.Vector2(InnerTexture.Width, InnerTexture.Height); }
        }

        public XnaTexture(byte[] data)
        {
            InnerTexture = Texture2D.FromStream(XnaWindow.GraphicsDevice, new MemoryStream(data));
        }

        public override void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
                InnerTexture.Dispose();
        }

        ~XnaTexture()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }
    }
}
