using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace System.IO
{
    public static class BinaryFileStreamExtensions
    {
        public static void Write(this BinaryWriter writer, Vector2 vec)
        {
            writer.Write(vec.X);
            writer.Write(vec.Y);
        }

        public static void Write(this BinaryWriter writer, Color color)
        {
            writer.Write(color.R);
            writer.Write(color.G);
            writer.Write(color.B);
            writer.Write(color.A);
        }

        public static Color ReadColor(this BinaryReader reader)
        {
            Color result = new Color();
            result.R = reader.ReadByte();
            result.G = reader.ReadByte();
            result.B = reader.ReadByte();
            result.A = reader.ReadByte();
            return result;
        }

        public static Vector2 ReadVector2(this BinaryReader reader)
        {
            Vector2 result;
            result.X = reader.ReadSingle();
            result.Y = reader.ReadSingle();
            return result;
        }
    }
}
