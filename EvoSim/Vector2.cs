using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoSim
{
    [Serializable]
    public struct Vector2
    {
        public float X;
        public float Y;

        public Vector2(float Value)
        {
            X = Value;
            Y = Value;
        }

        public Vector2(float InX, float InY)
        {
            X = InX;
            Y = InY;
        }

        public static Vector2 operator- (Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2 operator /(Vector2 a, float b)
        {
            return new Vector2(a.X / b, a.Y / b);
        }

        public static Vector2 operator *(Vector2 a, float b)
        {
            return new Vector2(a.X * b, a.Y * b);
        }

        public float LengthSquared()
        {
            return X*X + Y*Y;
        }

        private static Vector2 zero = new Vector2();

        public static Vector2 Zero
        {
            get { return zero; }
        }

        private static Vector2 unitX = new Vector2(1, 0);

        public static Vector2 UnitX
        {
            get { return unitX; }
        }

        private static Vector2 unitY = new Vector2(0, 1);

        public static Vector2 UnitY
        {
            get { return unitY; }
        }
    }
}
