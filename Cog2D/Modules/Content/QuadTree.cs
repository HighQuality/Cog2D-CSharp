using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog.Modules.Content
{
    public class QuadTree<T>
        where T : class
    {
        public int Size { get; private set; }

        public QuadTreeEntry<T> First,
            Last;

        public QuadTree<T> Parent,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight;
        public Vector2 Position { get; private set; }
        public Rectangle BoundingBox { get { return new Rectangle(Position, new Vector2(Size, Size)); } }

        public QuadTree(Vector2 position, int size)
            : this(null, position, size)
        {
        }

        private QuadTree(QuadTree<T> parent, Vector2 position, int size)
        {
            var logarithm = Math.Log(size, 2);
            if (logarithm != (int)logarithm)
                throw new ArgumentException("size must be a power of 2!");

            this.Parent = parent;
            this.Position = position;
            this.Size = size;
        }

        public QuadTree<T> Expand(Rectangle to)
        {
            if (Parent != null)
                throw new InvalidOperationException("This QuadTree has a parent!");
            if (BoundingBox.Contains(to))
                return this;

            Vector2 delta = to.Center - BoundingBox.Center;
            if (delta.X < 0f)
            {
                if (delta.Y < 0f)
                {
                    Parent = new QuadTree<T>(Position - new Vector2(Size, Size), Size * 2);
                    Parent.BottomRight = this;
                }
                else
                {
                    Parent = new QuadTree<T>(Position - new Vector2(Size, 0f), Size * 2);
                    Parent.TopRight = this;
                }
            }
            else
            {
                if (delta.Y < 0f)
                {
                    Parent = new QuadTree<T>(Position - new Vector2(0f, Size), Size * 2);
                    Parent.BottomLeft = this;
                }
                else
                {
                    Parent = new QuadTree<T>(Position, Size * 2);
                    Parent.TopLeft = this;
                }
            }
            return Parent.Expand(to);
        }

        public QuadTreeEntry<T> Insert(T obj, Rectangle boundingBox)
        {
            if (!BoundingBox.Contains(boundingBox))
                throw new Exception("Object can't fit in this quadtree!");
            return PInsert(obj, boundingBox);
        }

        private QuadTreeEntry<T> PInsert(T obj, Rectangle boundingBox)
        {
            int halfSize = Size / 2;

            if (boundingBox.Contains(new Rectangle(Position, new Vector2(halfSize, halfSize))))
            {
                if (TopLeft != null)
                    TopLeft = new QuadTree<T>(this, Position, halfSize);
                return TopLeft.PInsert(obj, boundingBox);
            }
            else if (boundingBox.Contains(new Rectangle(Position + new Vector2(halfSize, 0f), new Vector2(halfSize, halfSize))))
            {
                if (TopRight != null)
                    TopRight = new QuadTree<T>(this, Position + new Vector2(halfSize, 0f), halfSize);
                return TopRight.PInsert(obj, boundingBox);
            }
            else if (boundingBox.Contains(new Rectangle(Position + new Vector2(0f, halfSize), new Vector2(halfSize, halfSize))))
            {
                if (BottomLeft != null)
                    BottomLeft = new QuadTree<T>(this, Position + new Vector2(0f, halfSize), halfSize);
                return BottomLeft.PInsert(obj, boundingBox);
            }
            else if (boundingBox.Contains(new Rectangle(Position + new Vector2(halfSize, halfSize), new Vector2(halfSize, halfSize))))
            {
                if (BottomRight != null)
                    BottomRight = new QuadTree<T>(this, Position + new Vector2(halfSize, halfSize), halfSize);
                return BottomRight.PInsert(obj, boundingBox);
            }
            else
            {
                // It fits in here but in none of our children
                var entry = new QuadTreeEntry<T>(this, obj, boundingBox);
                if (First == null)
                {
                    First = entry;
                    Last = entry;
                }
                else
                {
                    Last.Next = entry;
                    entry.Previous = Last;
                    Last = entry;
                }
                return entry;
            }
        }
        
        public QuadTreeEntry<T> Move(QuadTreeEntry<T> entry, Rectangle boundingBox)
        {
            int halfSize = Size / 2;

            if (!BoundingBox.Contains(boundingBox))
            {
                // It no longer fits in us, move it to our parent
                if (Parent == null)
                {
                    Vector2 delta = boundingBox.Center - BoundingBox.Center;
                    if (delta.X > 0f)
                    {
                        if (delta.Y > 0f)
                        {
                            Parent = new QuadTree<T>(Position - new Vector2(Size, Size), Size * 2);
                            Parent.BottomRight = this;
                        }
                        else
                        {
                            Parent = new QuadTree<T>(Position - new Vector2(Size, 0f), Size * 2);
                            Parent.TopRight = this;
                        }
                    }
                    else
                    {
                        if (delta.Y > 0f)
                        {
                            Parent = new QuadTree<T>(Position - new Vector2(0f, Size), Size * 2);
                            Parent.BottomLeft = this;
                        }
                        else
                        {
                            Parent = new QuadTree<T>(Position, Size * 2);
                            Parent.TopLeft = this;
                        }
                    }
                }

                // Remove the entry from this QuadTree if it belongs here
                if (entry.Owner == this)
                    Remove(entry);
                // Move it into our parent
                return Parent.Move(entry, boundingBox);
            }
            else if (boundingBox.Contains(new Rectangle(Position, new Vector2(halfSize, halfSize))))
            {
                if (TopLeft != null)
                    TopLeft = new QuadTree<T>(this, Position, halfSize);
                return TopLeft.Move(entry, boundingBox);
            }
            else if (boundingBox.Contains(new Rectangle(Position + new Vector2(halfSize, 0f), new Vector2(halfSize, halfSize))))
            {
                if (TopRight != null)
                    TopRight = new QuadTree<T>(this, Position + new Vector2(halfSize, 0f), halfSize);
                return TopRight.Move(entry, boundingBox);
            }
            else if (boundingBox.Contains(new Rectangle(Position + new Vector2(0f, halfSize), new Vector2(halfSize, halfSize))))
            {
                if (BottomLeft != null)
                    BottomLeft = new QuadTree<T>(this, Position + new Vector2(0f, halfSize), halfSize);
                return BottomLeft.Move(entry, boundingBox);
            }
            else if (boundingBox.Contains(new Rectangle(Position + new Vector2(halfSize, halfSize), new Vector2(halfSize, halfSize))))
            {
                if (BottomRight != null)
                    BottomRight = new QuadTree<T>(this, Position + new Vector2(halfSize, halfSize), halfSize);
                return BottomRight.Move(entry, boundingBox);
            }
            else if (entry.Owner != this)
            {
                // It fits here and didn't originally belong here
                var newEntry = new QuadTreeEntry<T>(this, entry.Object, boundingBox);
                if (First == null)
                {
                    First = entry;
                    Last = entry;
                }
                else
                {
                    Last.Next = entry;
                    entry.Previous = Last;
                    Last = entry;
                }
                return newEntry;
            }
            else
            {
                // It's fine where it is
                entry.BoundingBox = boundingBox;
                return entry;
            }
        }

        public void Query(Vector2 position, Action<T> action)
        {
            if (!BoundingBox.Contains(position))
                return;

            var e = First;
            while (e != null)
            {
                if (e.BoundingBox.Contains(position))
                    action(e.Object);
                e = e.Next;
            }

            if (TopLeft != null)
                TopLeft.Query(position, action);
            if (TopRight != null)
                TopRight.Query(position, action);
            if (BottomLeft != null)
                BottomLeft.Query(position, action);
            if (BottomRight != null)
                BottomRight.Query(position, action);
        }

        public void Query(Rectangle rectangle, Action<T> action)
        {
            if (!rectangle.Intersects(BoundingBox))
                return;

            var e = First;
            while (e != null)
            {
                if (e.BoundingBox.Intersects(rectangle))
                    action(e.Object);
                e = e.Next;
            }

            if (TopLeft != null)
                TopLeft.Query(rectangle, action);
            if (TopRight != null)
                TopRight.Query(rectangle, action);
            if (BottomLeft != null)
                BottomLeft.Query(rectangle, action);
            if (BottomRight != null)
                BottomRight.Query(rectangle, action);
        }

        public List<T> Query(Rectangle rectangle)
        {
            List<T> l = new List<T>();
            Query(rectangle, o => l.Add(o));
            return l;
        }

        public List<T> Query(Vector2 position)
        {
            List<T> l = new List<T>();
            Query(position, o => l.Add(o));
            return l;
        }

        public T QuerySingle(Rectangle rectangle)
        {
            if (!rectangle.Intersects(BoundingBox))
                return null;

            var e = First;
            while (e != null)
            {
                if (e.BoundingBox.Intersects(rectangle))
                    return e.Object;
                e = e.Next;
            }

            T res = null;

            if (TopLeft != null)
                res = TopLeft.QuerySingle(rectangle);
            if (res != null)
                return res;

            if (TopRight != null)
                res = TopRight.QuerySingle(rectangle);
            if (res != null)
                return res;

            if (BottomLeft != null)
                res = BottomLeft.QuerySingle(rectangle);
            if (res != null)
                return res;

            if (BottomRight != null)
                res = BottomRight.QuerySingle(rectangle);
            return res;
        }

        public T QuerySingle(Vector2 position)
        {
            if (!BoundingBox.Contains(position))
                return null;

            var e = First;
            while (e != null)
            {
                if (e.BoundingBox.Contains(position))
                    return e.Object;
                e = e.Next;
            }

            T res = null;

            if (TopLeft != null)
                res = TopLeft.QuerySingle(position);
            if (res != null)
                return res;

            if (TopRight != null)
                res = TopRight.QuerySingle(position);
            if (res != null)
                return res;

            if (BottomLeft != null)
                res = BottomLeft.QuerySingle(position);
            if (res != null)
                return res;

            if (BottomRight != null)
                res = BottomRight.QuerySingle(position);
            return res;
        }

        public void Remove(QuadTreeEntry<T> entry)
        {
            if (entry.Owner != this)
                throw new InvalidOperationException("This entry does not belong to this QuadTree!");

            bool neither = true;
            if (entry == Last)
            {
                Last = entry.Previous;
                neither = false;
            }
            if (entry == First)
            {
                First = entry.Next;
                neither = false;
            }
            if (neither)
            {
                entry.Previous.Next = entry.Next;
            }
        }
    }
}
