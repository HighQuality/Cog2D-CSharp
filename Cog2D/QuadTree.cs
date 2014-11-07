using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public class QuadTree<T> : IBoundingBox
        where T : IBoundingBox
    {
        public int Size { get; private set; }
        public List<T> Objects = new List<T>();
        public QuadTree<T> TopLeft,
            TopRight,
            BottomLeft,
            BottomRight;
        public Vector2 Position { get; private set; }
        public Rectangle BoundingBox { get { return new Rectangle(Position, new Vector2(Size, Size)); } }

        public QuadTree(Vector2 position, int size)
        {
            var logarithm = Math.Log(size, 2);
            if (logarithm != (int)logarithm)
                throw new ArgumentException("size must be a power of 2!");

            this.Position = position;
            this.Size = size;
        }

        public bool Insert(T obj)
        {
            if (!BoundingBox.Contains(obj.BoundingBox))
                throw new Exception("Object can't fit in this quadtree!");
            var b = PInsert(obj);
            if (!b)
                throw new Exception("PInsert returned false!");
            return b;
        }

        private bool PInsert(T obj)
        {
            int halfSize = Size / 2;

            if (obj.BoundingBox.Contains(new Rectangle(Position, new Vector2(halfSize, halfSize))))
            {
                if (TopLeft != null)
                    TopLeft = new QuadTree<T>(Position, halfSize);
                return TopLeft.PInsert(obj);
            }
            else if (obj.BoundingBox.Contains(new Rectangle(Position + new Vector2(halfSize, 0f), new Vector2(halfSize, halfSize))))
            {
                if (TopRight != null)
                    TopRight = new QuadTree<T>(Position + new Vector2(halfSize, 0f), halfSize);
                return TopRight.PInsert(obj);
            }
            else if (obj.BoundingBox.Contains(new Rectangle(Position + new Vector2(0f, halfSize), new Vector2(halfSize, halfSize))))
            {
                if (BottomLeft != null)
                    BottomLeft = new QuadTree<T>(Position + new Vector2(0f, halfSize), halfSize);
                return BottomLeft.PInsert(obj);
            }
            else if (obj.BoundingBox.Contains(new Rectangle(Position + new Vector2(halfSize, halfSize), new Vector2(halfSize, halfSize))))
            {
                if (BottomRight != null)
                    BottomRight = new QuadTree<T>(Position + new Vector2(halfSize, halfSize), halfSize);
                return BottomRight.PInsert(obj);
            }
            else
            {
                Objects.Add(obj);
                return true;
            }
        }

        public void Query(Rectangle rectangle, Action<T> match)
        {
            if (rectangle.Intersects(BoundingBox))
            for (int i = Objects.Count - 1; i >= 0; i--)
                if (Objects[i].BoundingBox.Intersects(rectangle))
                    match(Objects[i]);
            if (TopLeft != null)
                TopLeft.Query(rectangle, match);
            if (TopRight != null)
                TopRight.Query(rectangle, match);
            if (BottomLeft != null)
                BottomLeft.Query(rectangle, match);
            if (BottomRight != null)
                BottomRight.Query(rectangle, match);
        }

        public List<T> Query(Rectangle rectangle)
        {
            List<T> l = new List<T>();
            Query(rectangle, o => l.Add(o));
            return l;
        }
    }
}
