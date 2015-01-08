using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D3DRenderer
{
    public class D3DTexture : Texture
    {
        public SlimDX.Direct3D11.Texture2D InnerTexture;

        internal D3DTexture(byte[] data)
        {
            InnerTexture = SlimDX.Direct3D11.Texture2D.FromMemory(D3DWindow.Device, data);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                InnerTexture.Dispose();
            }
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~D3DTexture()
        {
            Dispose(false);
        }
    }
}
