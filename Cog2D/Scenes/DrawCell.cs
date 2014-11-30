using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Scenes
{
    public struct DrawCell
    {
        public const int DrawCellSize = 512;

        public int X,
            Y;

        public DrawCell(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
