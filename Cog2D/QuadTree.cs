using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog
{
    public class QuadTree
    {
        public QuadTree[] Children = new QuadTree[4];

        public QuadTree(int size)
        {
            var logarithm = Math.Log(size, 2);
            if (logarithm != (int)logarithm)
                throw new ArgumentException("size must be a power of 2!");
        }
    }
}
