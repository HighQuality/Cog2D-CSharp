using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square
{
    public class Image
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        private Color[] colors;

        public Color this[int x, int y]
        {
            get { if (x < 0 || x >= Width) throw new ArgumentOutOfRangeException("x"); if (y < 0 || y >= Width) throw new ArgumentOutOfRangeException("y"); return colors[x + y * Width]; }
            set { if (x < 0 || x >= Width) throw new ArgumentOutOfRangeException("x"); if (y < 0 || y >= Width) throw new ArgumentOutOfRangeException("y"); colors[x + y * Height] = value; }
        }

        public Image(int width, int height)
        {
            Width = width;
            Height = height;
            colors = new Color[Width * Height];
        }

        public void SetColor(int x, int y, Color color)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException("x");
            if (y < 0 || y >= Width)
                throw new ArgumentOutOfRangeException("y");
            colors[x + y * Width] = color;
        }

        public Color GetColor(int x, int y)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException("x");
            if (y < 0 || y >= Width)
                throw new ArgumentOutOfRangeException("y");
            return colors[x + y * Width];
        }
    }
}
