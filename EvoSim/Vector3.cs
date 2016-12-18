using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvoNet;

namespace EvoSim
{
    public struct Vector3
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3(float Value)
        {
            X = Value;
            Y = Value;
            Z = Value;
        }

        public Vector3(float InX, float InY, float InZ)
        {
            X = InX;
            Y = InY;
            Z = InZ;
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3 operator /(Vector3 a, float b)
        {
            return new Vector3(a.X / b, a.Y / b, a.Z / b);
        }

        public float LengthSquared()
        {
            return X * X + Y * Y + Z*Z;
        }

        public float Length()
        {
            return Mathf.Sqrt(LengthSquared());
        }
    }
}
