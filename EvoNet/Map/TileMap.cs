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
          renderRectangles[x, y] = new Rectangle((int)(x * inTileSize),(int) (y * inTileSize), (int)inTileSize, (int)inTileSize);
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

    }

    public void Draw(GameTime deltaTime)
    {
      // Todo Camera
      spriteBatch.Begin();
      for (int x = 0; x < Width; x++)
      {
        for (int y = 0; y < Height; y++)
        {
          spriteBatch.Draw(EvoGame.WhiteTexture, renderRectangles[x, y], types[x, y] == TileType.Land ? Color.Brown : Color.Blue);
        }
      }
      spriteBatch.End();
    }


  }
}
