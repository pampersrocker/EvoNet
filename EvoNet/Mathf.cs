using System;

namespace EvoNet
{
    public class Mathf
    {
        public const float PI = 3.1415926535f;
        public const float DEGREETORAD = PI / 180.0f;
        public const float RADTODEGREE = 180.0f / PI;
        public static float Abs(float value)
        {
            return (float)Math.Abs(value);
        }
        public static float Sin(float value)
        {
            return (float)Math.Sin(value);
        }

        public static float Cos(float value)
        {
            return (float)Math.Cos(value);
        }

        public static float Tan(float value)
        {
            return (float)Math.Tan(value);
        }

        public static float Asin(float value)
        {
            return (float)Math.Asin(value);
        }

        public static float Acos(float value)
        {
            return (float)Math.Acos(value);
        }

        public static float Atan(float value)
        {
            return (float)Math.Atan(value);
        }

        public static float Sigmoid(float x)
        {
            float et = (float)Math.Pow(Math.E, x);
            return et / (1 + et);
        }

        public static float InterpolateCosine(float a, float b, float t)
        {
            float t2 = (1 - Cos(t * PI)) / 2;
            return (a * (1 - t2) + b * t2);
        }
    }
}
