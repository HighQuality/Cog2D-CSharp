using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square
{
    public struct Vector2
    {
        /// <summary>
        /// The X-coordinate of the Vector
        /// </summary>
        public float X;

        /// <summary>
        /// The Y-coordinate of the Vector
        /// </summary>
        public float Y;

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
    }
}
