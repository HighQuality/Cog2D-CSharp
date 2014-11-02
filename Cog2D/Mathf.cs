using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog
{
    public static class Mathf
    {
        public const float Pi = (float)Math.PI;

        public static float Min(float value1, float value2)
        {
            return value1 < value2 ? value1 : value2;
        }

        public static float Max(float value1, float value2)
        {
            return value1 > value2 ? value1 : value2;
        }

        public static float Pow(float x, float y)
        {
            return (float)Math.Pow(x, y);
        }

        public static float Sqrt(float value)
        {
            return (float)Math.Sqrt(value);
        }

        public static float Sin(float value)
        {
            return (float)Math.Sin(value);
        }

        public static float Cos(float value)
        {
            return (float)Math.Cos(value);
        }
    }
}
