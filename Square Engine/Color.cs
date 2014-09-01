using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square
{
    public struct Color
    {
        public static Color White = new Color(255, 255, 255),
            CornflowerBlue = new Color(100, 149, 237);

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
    }
}
