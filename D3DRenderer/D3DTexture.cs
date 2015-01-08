using Cog;
using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D3DRenderer
{
    public class D3DTexture : Texture
    {
        internal SlimDX.Direct3D11.Texture2D InnerTexture;
        internal SlimDX.Direct3D11.ShaderResourceView ResourceView;
        private Vector2 _size;
        public override Vector2 Size { get { return _size; } }

        internal D3DTexture(byte[] data)
        {
            InnerTexture = SlimDX.Direct3D11.Texture2D.FromMemory(D3DWindow.Device, data);
            ResourceView = new SlimDX.Direct3D11.ShaderResourceView(D3DWindow.Device, InnerTexture);
            _size = new Vector2(InnerTexture.Description.Width, InnerTexture.Description.Height);
        }

        internal D3DTexture(Image image)
        {
            var data = new SlimDX.DataStream(image.Width * image.Height * sizeof(float) * 4, true, true);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var color = image.GetColor(x, y);

                    data.Write((float)color.R / 255f);
                    data.Write((float)color.G / 255f);
                    data.Write((float)color.B / 255f);
                    data.Write((float)color.A / 255f);
                }
            }
            data.Position = 0;

            InnerTexture = new SlimDX.Direct3D11.Texture2D(D3DWindow.Device, new SlimDX.Direct3D11.Texture2DDescription
            {
                Width = image.Width,
                Height = image.Height,
                ArraySize = 1,
                BindFlags = SlimDX.Direct3D11.BindFlags.ShaderResource,
                MipLevels = 0,
                Format = SlimDX.DXGI.Format.R8G8B8A8_UNorm,
                Usage = SlimDX.Direct3D11.ResourceUsage.Default
            }, new SlimDX.DataRectangle(image.Width * sizeof(float) * 4, data));

            ResourceView = new SlimDX.Direct3D11.ShaderResourceView(D3DWindow.Device, InnerTexture);

            _size = new Vector2(image.Width, image.Height);
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
