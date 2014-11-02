using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog
{
    public struct Vector2
    {
        public static Vector2 Zero = new Vector2(0f, 0f),
            One = new Vector2(1f, 1f),
            Left = new Vector2(-1f, 0f),
            Right = new Vector2(1f, 0f),
            Up = new Vector2(0f, -1f),
            Down = new Vector2(0f, 1f);

        /// <summary>
        /// The X-coordinate of the Vector
        /// </summary>
        public float X;

        /// <summary>
        /// The Y-coordinate of the Vector
        /// </summary>
        public float Y;

        public float Length
        {
            get
            {
                return Mathf.Sqrt(Mathf.Pow(X, 2f) + Mathf.Pow(Y, 2f));
            }
        }

        /// <summary>
        /// Instantiates a Vector2 with the specified parameters
        /// </summary>
        /// <param name="x">The X-coordinate</param>
        /// <param name="y">The Y-coordinate</param>
        public Vector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2 Rotate(Angle angle)
        {
            var r = angle.Radian;
            return new Vector2(X * Mathf.Cos(r) - Y * Mathf.Sin(r),
                X * Mathf.Sin(r) + Y * Mathf.Cos(r));
        }

        public static Vector2 operator +(Vector2 first, Vector2 second)
        {
            return new Vector2(first.X + second.X, first.Y + second.Y);
        }
        public static Vector2 operator -(Vector2 first, Vector2 second)
        {
            return new Vector2(first.X - second.X, first.Y - second.Y);
        }
        public static Vector2 operator *(Vector2 first, Vector2 second)
        {
            return new Vector2(first.X * second.X, first.Y * second.Y);
        }
        public static Vector2 operator /(Vector2 first, Vector2 second)
        {
            return new Vector2(first.X / second.X, first.Y / second.Y);
        }

        public static Vector2 operator /(Vector2 first, float second)
        {
            return new Vector2(first.X / second, first.Y / second);
        }

        public static Vector2 operator *(Vector2 first, float second)
        {
            return new Vector2(first.X * second, first.Y * second);
        }

        public static bool operator ==(Vector2 first, Vector2 second)
        {
            return first.X == second.X && first.Y == second.Y;
        }

        public static bool operator !=(Vector2 first, Vector2 second)
        {
            return first.X != second.X || first.Y != second.Y;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[Vector2]{{ X : {0}; Y : {1} }}", X, Y);
        }
    }
}
