using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet.Map
{

    public enum TileType
    {
        None,
        Land,
        Water
    }
    public struct Tile
    {
        public float food;
        public TileType type;
        public Point position;


        public Tile(Point inPosition, TileType inType, float inFood = 0.0f)
        {
            position = inPosition;
            type = inType;
            food = inFood;
        }

        public bool IsLand()
        {
            return type == TileType.Land;
        }

        public bool IsWater()
        {
            return type == TileType.Water;
        }
    }
}
