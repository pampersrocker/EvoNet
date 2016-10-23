using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet.Map
{
    public class TileMap
    {
        float[,] foodValues;
        TileType[,] types;
        Rectangle[,] renderRectangles;

        float tileSize;

        SpriteBatch spriteBatch;
        public Camera Camera { get; set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public float[,] FoodValues
        {
            get
            {
                return foodValues;
            }

            set
            {
                foodValues = value;
            }
        }

        public TileType[,] Types
        {
            get
            {
                return types;
            }

            set
            {
                types = value;
            }
        }

        public TileMap(int width, int height, float inTileSize)
        {
            Width = width;
            Height = height;
            foodValues = new float[width, height];
            types = new TileType[width, height];
            renderRectangles = new Rectangle[width, height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    renderRectangles[x, y] = new Rectangle((int)(x * inTileSize), (int)(y * inTileSize), (int)inTileSize, (int)inTileSize);
                }
            }
            tileSize = inTileSize;
        }

        public Tile GetTileInfo(int x, int y)
        {
            return new Tile(new Point(x, y), types[x, y], foodValues[x, y]);
        }

        public Tile GetTileInfo(Point position)
        {
            return new Tile(position, types[position.X, position.Y], foodValues[position.X, position.Y]);
        }

        public Tile GetTileAtWorldPosition(Vector2 position)
        {
            position /= tileSize;
            return GetTileInfo(position.ToPoint());
        }

        public void Initialize(EvoGame game)
        {
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
        }

        public void Update(GameTime deltaTime)
        {
            for(int i = 0; i<Width; i++)
            {
                for(int k = 0; k<Height; k++)
                {
                    if(IsFertile(i, k))
                    {
                        Grow(i, k);
                    }
                }
            }
        }

        public void Grow(int x, int y)
        {
            foodValues[x, y] += 0.3f;
            if (foodValues[x, y] > 100) foodValues[x, y] = 100;
        }

        public bool IsFertileToNeighbors(int x, int y)
        {
            if(x < 0 || y < 0 || x >= Width || y >= Height)
            {
                return false; //If out of bounds
            }
            if(types[x, y] == TileType.Water)
            {
                return true;
            }
            if(types[x, y] == TileType.Land && foodValues[x, y] > 50)
            {
                return true;
            }
            return false;
        }

        public bool IsFertile(int x, int y)
        {
            if(types[x, y] == TileType.Land)
            {
                if(foodValues[x, y] > 50)
                {
                    return true;
                }
                if (IsFertileToNeighbors(x - 1, y))
                {
                    return true;
                }
                if (IsFertileToNeighbors(x + 1, y))
                {
                    return true;
                }
                if (IsFertileToNeighbors(x, y - 1))
                {
                    return true;
                }
                if (IsFertileToNeighbors(x, y + 1))
                {
                    return true;
                }
            }

            return false;
        }

        public void Draw(GameTime deltaTime)
        {
            // Todo Camera
            Matrix? UsedMatrix = null;
            if (Camera != null)
            {
                UsedMatrix = Camera.Matrix;
            }
            spriteBatch.Begin(transformMatrix: UsedMatrix);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Color color = new Color();
                    if(types[x, y] == TileType.Water)
                    {
                        color = Color.Blue;
                    }else if(types[x, y] == TileType.Land)
                    {
                        float bleach = 1 - foodValues[x, y] / 100f;
                        color = new Color(bleach, 1, bleach);
                    }else if(types[x, y] == TileType.None)
                    {
                        color = Color.Black;
                    }
                    spriteBatch.Draw(EvoGame.WhiteTexture, renderRectangles[x, y], color);
                }
            }
            spriteBatch.End();
        }


    }
}
