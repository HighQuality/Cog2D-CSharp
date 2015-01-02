using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog
{
    public struct Color
    {
        public static Color White = new Color(255, 255, 255),
            CornflowerBlue = new Color(100, 149, 237),
            Blue = new Color(0, 0, 255, 255),
            Red = new Color(255, 0, 0, 255),
            Green = new Color(0, 255, 0, 255),
            Gray = new Color(128, 128, 128, 255),
            Black = new Color(0, 0, 0),
            Yellow = new Color(255, 255, 0),
            Purple = new Color(255, 0, 255),
            Cyan = new Color(0, 255, 255);

        public int R,
            G,
            B,
            A;

        public Color(int r, int g, int b)
            : this(r, g, b, 255)
        {
        }

        public Color(int r, int g, int b, int a)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }

        public Color Transition(Color to, float progress)
        {
            return new Color((int)(((float)to.R - (float)R) * progress) + R,
                (int)(((float)to.G - (float)G) * progress) + G,
                (int)(((float)to.B - (float)B) * progress) + B,
                (int)(((float)to.A - (float)A) * progress) + A);
        }

        public static Color operator*(Color first, float second)
        {
            return new Color(first.R, first.G, first.B, (int)((float)first.A * second));
        }

        public static Color operator+(Color first, Color second)
        {
            return new Color(first.R + second.R, first.G + second.G, first.B + second.B, first.A + second.A);
        }

        public static Color operator -(Color first, Color second)
        {
            return new Color(first.R - second.R, first.G - second.G, first.B - second.B, first.A - second.A);
        }
    }
}
