using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public class QuadTreeEntry<T>
        where T : class
    {
        public QuadTreeEntry<T> Previous,
            Next;

        public QuadTree<T> Owner { get; private set; }
        public T Object { get; private set; }
        public Rectangle BoundingBox { get; internal set; }

        internal QuadTreeEntry(QuadTree<T> owner, T obj, Rectangle boundingBox)
        {
            this.Owner = owner;
            this.Object = obj;
            this.BoundingBox = boundingBox;
        }
    }
}
