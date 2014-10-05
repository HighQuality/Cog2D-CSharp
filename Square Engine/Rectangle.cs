using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square
{
    public struct Rectangle
    {
        private Vector2 _topLeft,
            _size;

        public Vector2 TopLeft { get { return _topLeft; } set { _topLeft = value; } }
        public Vector2 TopRight { get { return TopLeft + new Vector2(Size.X, 0f); } }
        public Vector2 BottomLeft { get { return TopLeft + new Vector2(0f, Size.Y); } }
        public Vector2 BottomRight { get { return TopLeft + Size; } }
        public Vector2 Center { get { return TopLeft + Size / 2f; } }
        public Vector2 Size { get { return _size; } set { _size = value; } }

        public Rectangle(Vector2 topLeft, Vector2 size)
        {
            this._topLeft = topLeft;
            this._size = size;
        }

        public bool Intersects(Rectangle other)
        {
            return (TopLeft <= other.BottomRight && BottomRight > other.TopLeft);
        }

        public bool Contains(Rectangle other)
        {
            return other.TopLeft >= TopLeft && other.BottomRight <= BottomRight;
        }
    }
}
