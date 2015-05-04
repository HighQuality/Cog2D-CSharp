using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog
{
    public struct Rectangle
    {
        private Vector2 _topLeft,
            _size;

        public float Left { get { return _topLeft.X; } set { _topLeft.X = value; } }
        public float Top { get { return _topLeft.Y; } set { _topLeft.Y = value; } }
        public float Width { get { return _size.X; } set { _size.X = value; } }
        public float Height { get { return _size.Y; } set { _size.Y = value; } }
        public float Right { get { return Left + Width; } }
        public float Bottom { get { return Top + Height; } }

        public Vector2 TopLeft { get { return _topLeft; } set { _topLeft = value; } }
        public Vector2 TopRight { get { return TopLeft + new Vector2(Size.X, 0f); } }
        public Vector2 BottomLeft { get { return TopLeft + new Vector2(0f, Size.Y); } }
        public Vector2 BottomRight { get { return TopLeft + Size; } }
        public Vector2 Center { get { return TopLeft + Size / 2f; } }
        public Vector2 Size { get { return _size; } set { _size = value; } }

        public Rectangle(float left, float top, float width, float height)
        {
            _topLeft = new Vector2(left, top);
            _size = new Vector2(width, height); 
        }

        public Rectangle(Vector2 topLeft, Vector2 size)
        {
            this._topLeft = topLeft;
            this._size = size;
        }

        public Rectangle OverlayOf(Rectangle other)
        {
            var topLeft = new Vector2(Mathf.Max(Left, other.Left), Mathf.Max(Top, other.Top));
            var bottomRight = new Vector2(Mathf.Min(Right, other.Right), Mathf.Min(Bottom, other.Bottom));
            var size = bottomRight - topLeft;

            topLeft.X -= Mathf.Min(Left, other.Left);
            topLeft.Y -= Mathf.Min(Top, other.Top);
            return new Rectangle(topLeft,
                size);
        }

        public bool Intersects(Rectangle other)
        {
            return (BottomRight.X >= other.TopLeft.X && BottomRight.Y >= other.TopLeft.Y) &&
                (TopLeft.X < other.BottomRight.X && TopLeft.Y < other.BottomRight.Y);
        }

        public bool Contains(Rectangle other)
        {
            return (other.Left >= Left && other.Top >= Top) &&
                (other.Right < Right && other.Bottom < Bottom);
        }

        public bool Contains(Vector2 point)
        {
            return point.X >= TopLeft.X && point.Y >= TopLeft.Y && point.X < TopLeft.X + Size.X && point.Y < TopLeft.Y + Size.Y;
        }
    }
}
