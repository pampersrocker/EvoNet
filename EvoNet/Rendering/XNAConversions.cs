using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet.Rendering
{
    public static class XNAConversions
    {
        public static Microsoft.Xna.Framework.Vector2 ToXNA(this EvoSim.Vector2 vec)
        {
            return new Microsoft.Xna.Framework.Vector2(vec.X, vec.Y);
        }

        public static Microsoft.Xna.Framework.Color ToXNA(this EvoSim.Color color)
        {
            return new Microsoft.Xna.Framework.Color(color.R, color.G, color.B, color.A);
        }
    }
}
