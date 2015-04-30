using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;

namespace Cog
{
    public class Image
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Color[] Data;

        public Color this[int x, int y]
        {
            get { if (x < 0 || x >= Width) throw new ArgumentOutOfRangeException("x"); if (y < 0 || y >= Width) throw new ArgumentOutOfRangeException("y"); return Data[x + y * Width]; }
            set { if (x < 0 || x >= Width) throw new ArgumentOutOfRangeException("x"); if (y < 0 || y >= Width) throw new ArgumentOutOfRangeException("y"); Data[x + y * Height] = value; }
        }

        public Image(int width, int height)
        {
            Width = width;
            Height = height;
            Data = new Color[Width * Height];
        }

        public Image(string filename)
            : this(File.ReadAllBytes(filename))
        {
        }

        public Image(byte[] data)
        {
            using (var memStream = new MemoryStream(data))
            {
                using (var bitmap = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(memStream))
                {
                    Width = bitmap.Width;
                    Height = bitmap.Height;
                    Data = new Color[Width * Height];

                    BitmapData bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                    int bytesPerPixel = Bitmap.GetPixelFormatSize(PixelFormat.Format32bppArgb) / 8;
                    byte[] pixels = new byte[bitmapData.Stride * bitmap.Height];
                    Marshal.Copy(bitmapData.Scan0, pixels, 0, pixels.Length);

                    for (int y = 0; y < bitmapData.Height; y++)
                    {
                        int currentLine = y * bitmapData.Stride;
                        for (int x = 0; x < bitmapData.Width; x++)
                        {
                            int pixelPosition = currentLine + x * bytesPerPixel;

                            int blue = pixels[pixelPosition];
                            int green = pixels[pixelPosition + 1];
                            int red = pixels[pixelPosition + 2];
                            int alpha = pixels[pixelPosition + 3];

                            Data[x + y * Width] = new Color(red, green, blue, alpha);
                        }
                    }

                    bitmap.UnlockBits(bitmapData);
                }   
            }
        }

        public void SetColor(int x, int y, Color color)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException("x");
            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException("y");
            Data[x + y * Width] = color;
        }

        public Color GetColor(int x, int y)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException("x");
            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException("y");
            return Data[x + y * Width];
        }

        public Bitmap ToBitmap()
        {
            var bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

            BitmapData bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            int bytesPerPixel = Bitmap.GetPixelFormatSize(PixelFormat.Format32bppArgb) / 8;
            byte[] pixels = new byte[bitmapData.Stride * bitmap.Height];

            for (int y = 0; y < Height; y++)
            {
                int currentLine = y * bitmapData.Stride;
                for (int x = 0; x < Width; x++)
                {
                    int pixelPosition = currentLine + x * bytesPerPixel;

                    var pixel = GetColor(x, y);

                    pixels[pixelPosition] = (byte)pixel.B;
                    pixels[pixelPosition + 1] = (byte)pixel.G;
                    pixels[pixelPosition + 2] = (byte)pixel.R;
                    pixels[pixelPosition + 3] = (byte)pixel.A;
                }
            }

            Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }
    }
}
