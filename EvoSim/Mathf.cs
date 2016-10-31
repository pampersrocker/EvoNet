using System;
using System.Collections.Generic;

namespace EvoNet
{
    public class Mathf
    {
        public const float PI = 3.1415926535f;
        public const float DEGREETORAD = PI / 180.0f;
        public const float RADTODEGREE = 180.0f / PI;
        public static float Sqrt(float value)
        {
            return (float)Math.Sqrt(value);
        }
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
            return (et / (1 + et))*2 - 1;
        }

        public static float InterpolateCosine(float a, float b, float t)
        {
            float t2 = (1 - Cos(t * PI)) / 2;
            return (a * (1 - t2) + b * t2);
        }

        public static float Clamp01(float value)
        {
            if (value < 0) return 0;
            if (value > 1) return 1;
            return value;
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public static float ClampNegPos(float value)
        {
            if (value < -1) return -1;
            if (value > 1) return 1;
            return value;
        }

        public static int ClampColorValue(int val)
        {
            if (val < 0) return 0;
            if (val > 255) return 255;
            return val;
        }

        public static float Max(List<float> list)
        {
            float max = float.NegativeInfinity;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] > max)
                {
                    max = list[i];
                }
            }
            return max;
        }

        public static float Min(List<float> list)
        {
            float min = float.PositiveInfinity;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] < min)
                {
                    min = list[i];
                }
            }
            return min;
        }

        public static float Max(float a, float b)
        {
            return a > b ? a : b;
        }

        public static float Min(float a, float b)
        {
            return a > b ? b : a;
        }
    }
}
