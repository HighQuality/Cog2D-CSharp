using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog
{
    public struct Angle
    {
        private float degree;

        public float Degree { get { return degree; } set { degree = value; } }
        public float Radian { get { return degree / 180f * (float)Math.PI; } set { degree = value / (float)Math.PI * 180f; } }
        public Angle Normalized { get { return new Angle(NormalizedDegree); } }
        public float NormalizedDegree { get { var val = degree % 360f; if (val < 0) val = 360f + val; return val; } }
        public float NormalizedRadian { get { return NormalizedDegree / 180f * (float)Math.PI; } }
        public Vector2 Unit { get { return new Vector2((float)Math.Cos(Radian), (float)Math.Sin(Radian)); } }

        public Angle(Vector2 vector)
            : this((float)Math.Atan2(vector.Y, vector.X) / (float)Math.PI * 180f)
        {
            Normalize();
        }

        public Angle(float degree)
        {
            this.degree = degree;
        }

        public static Angle operator+(Angle first, Angle second)
        {
            return new Angle(first.degree + second.degree);
        }

        public static Angle operator-(Angle first, Angle second)
        {
            return new Angle(first.degree - second.degree);
        }

        public void Normalize()
        {
            degree = NormalizedDegree;
        }

        public static Angle FromVector(Vector2 vector)
        {
            return new Angle(vector);
        }
        
        public static Angle FromDegree(float degree)
        {
            return new Angle(degree);
        }

        public static Angle FromRadian(float radian)
        {
            return new Angle(radian / (float)Math.PI * 180f);
        }

        public static Angle DegreeBetween(Vector2 first, Vector2 second)
        {
            return Angle.FromRadian((float)Math.Atan2(first.Y - second.Y, second.X - first.X)).Normalized;
        }
    }
}
