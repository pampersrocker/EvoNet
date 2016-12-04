using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EvoSim.Serialization
{
    public static class SerializationInfoExtensions
    {
        public static void AddVector2(this SerializationInfo info, string Key, Vector2 value)
        {
            info.AddValue(Key + "_X", value.X);
            info.AddValue(Key + "_Y", value.Y);
        }

        public static Vector2 GetVector2(this SerializationInfo info, string Key)
        {
            float x = info.GetSingle(Key + "_X");
            float y = info.GetSingle(Key + "_Y");
            return new Vector2(x, y);
        }

        public static void AddColor(this SerializationInfo info, string Key, Color value)
        {
            info.AddValue(Key + "_A", value.A);
            info.AddValue(Key + "_R", value.R);
            info.AddValue(Key + "_G", value.G);
            info.AddValue(Key + "_B", value.B);
        }

        public static Color GetColor(this SerializationInfo info, string Key)
        {
            Color value = new Color();
            value.A = info.GetByte(Key + "_A");
            value.R = info.GetByte(Key + "_R");
            value.G = info.GetByte(Key + "_G");
            value.B = info.GetByte(Key + "_B");
            return value;
        }
    }
}
