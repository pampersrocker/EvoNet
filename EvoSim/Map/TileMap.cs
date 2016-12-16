using EvoSim;
using EvoSim.Tasks;
using EvoSim.ThreadingHelper;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace EvoNet.Map
{
  [Serializable]
  public class TileMap : UpdateModule
  {
    public const float MAXIMUMFOODPERTILE = 100;

    private float[,] foodValues_;
    private TileType[,] types;
    public TileType[,] Types
    {
      get { return types; }
    }



    float tileSize;
    public float TileSize
    {
      get { return tileSize; }
    }
    public List<float> FoodRecord = new List<float>();


    public int Width { get; private set; }
    public int Height { get; private set; }

    private object foodValuesLock = new object();
    public float[,] FoodValues
    {
      get
      {
        lock (foodValuesLock)
        {
          return foodValues_;
        }
      }
      set
      {
        lock (foodValuesLock)
        {
          foodValues_ = value;
        }
      }
    }

    public void SetTileType(int x, int y, TileType tt)
    {
      types[x, y] = tt;
      if (tt != TileType.Land)
      {
        FoodValues[x, y] = 0;
      }
    }

    public bool SetFoodValue(int x, int y, float foodValue)
    {
      if (x >= 0 && x < Width && y >= 0 && y < Height)
      {

        if (types[x, y] == TileType.Land)
        {
          FoodValues[x, y] = foodValue;
          return true;
        }
      }
      return false;
    }

    public override bool WantsFastForward
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Creates a new tilemap
    /// </summary>
    /// <param name="width">Number of tiles in horizontal direction</param>
    /// <param name="height">Number of tiles in vertical direction</param>
    /// <param name="inTileSize">Width and Height of a Tile</param>
    /// <param name="renderTextureTileFactor">How many tiles use a single texture until it wraps?
    /// Setting this to a higher value reduces tiled look</param>
    public TileMap(int width, int height, float inTileSize)
    {
      Width = width;
      Height = height;
      FoodValues = new float[width, height];
      types = new TileType[width, height];


      for (int x = 0; x < Width; x++)
      {
        for (int y = 0; y < Height; y++)
        {
          FoodValues[x, y] = MAXIMUMFOODPERTILE;
        }
      }
      tileSize = inTileSize;
    }

    public Tile GetTileInfo(int x, int y)
    {
      return new Tile(new Point(x, y), types[x, y], FoodValues[x, y]);
    }

    public Tile GetTileInfo(Point position)
    {
      if (position.X < 0 || position.X > Width - 1 || position.Y < 0 || position.Y > Height - 1)
      {
        return new Tile(position, TileType.None, 0);
      }
      return new Tile(position, types[position.X, position.Y], FoodValues[position.X, position.Y]);
    }

    public Tile GetTileAtWorldPosition(Vector2 position)
    {
      position /= tileSize;
      return GetTileInfo(position.ToPoint());
    }

    public float EatOfTile(int x, int y, float eatAmount)
    {
      float foodVal = FoodValues[x, y];
      if (foodVal > eatAmount)
      {
        FoodValues[x, y] -= eatAmount;
        return eatAmount;
      }
      else
      {
        FoodValues[x, y] = 0;
        return foodVal;
      }
    }

    public float GetWorldWidth()
    {
      return Width * tileSize;
    }

    public float GetWorldHeight()
    {
      return Height * tileSize;
    }

    public float CalculateFoodAvailable()
    {
      float food = 0;
      for (int i = 0; i < Width; i++)
      {
        for (int k = 0; k < Height; k++)
        {
          food += FoodValues[i, k];
        }
      }
      return food;
    }

    [NonSerialized]
    public ThreadTaskGroup growGroup;

    public override void Initialize(Simulation game)
    {
      base.Initialize(game);

      growGroup = new ThreadTaskGroup();
      int divider = 4;
      int numTilesPerTaskX = Math.Max(1, Width / divider);
      int numTilesPerTaskY = Math.Max(1, Height / divider);
      for (int taskX = 0; taskX < (Width / numTilesPerTaskX); taskX++)
      {
        for (int taskY = 0; taskY < (Height / numTilesPerTaskY); taskY++)
        {
          int closureTaskX = taskX;
          int closureTaskY = taskY;
          Action<Simulation, GameTime> action = (Simulation sim, GameTime time) =>
          {
            float deltaTime = (float)time.ElapsedGameTime.TotalSeconds;
            for (int tileOffsetX = 0; tileOffsetX < numTilesPerTaskX; tileOffsetX++)
            {
              for (int tileOffsetY = 0; tileOffsetY < numTilesPerTaskY; tileOffsetY++)
              {
                int tileIndexX = closureTaskX * numTilesPerTaskX + tileOffsetX;
                int tileIndexY = closureTaskY * numTilesPerTaskY + tileOffsetY;
                if (tileIndexX < Width && tileIndexY < Height)
                {
                  if (IsFertile(tileIndexX, tileIndexY))
                  {
                    Grow(tileIndexX, tileIndexY, deltaTime);
                  }
                }
              }
            }
          };
          SimpleSimulationTask task = new SimpleSimulationTask(simulation, action);
          growGroup.AddTask(task);

        }
      }
      growGroup.AddTask(new SimpleSimulationTask(simulation, (sim, time) => { FoodRecord.Add(CalculateFoodAvailable()); }));
      simulation.TaskManager.AddGroup(growGroup);



    }

    protected override void Update(GameTime deltaTime)
    {
    }

    public void Grow(int x, int y, float fixedDeltaTime)
    {
      FoodValues[x, y] += 20f * fixedDeltaTime;
      if (FoodValues[x, y] > MAXIMUMFOODPERTILE) FoodValues[x, y] = MAXIMUMFOODPERTILE;
    }

    public bool IsFertileToNeighbors(int x, int y)
    {
      if (x < 0 || y < 0 || x >= Width || y >= Height)
      {
        return false; //If out of bounds
      }
      if (types[x, y] == TileType.Water)
      {
        return true;
      }
      if (types[x, y] == TileType.Land && FoodValues[x, y] > 50)
      {
        return true;
      }
      return false;
    }

    public bool IsFertile(int x, int y)
    {
      if (types[x, y] == TileType.Land)
      {
        if (FoodValues[x, y] > 50)
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

    public void SerializeToFile(string fileName)
    {
      FileStream file = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write);
      IFormatter formatter = new BinaryFormatter();
      formatter.Serialize(file, this);
      file.Close();
    }

    public static TileMap DeserializeFromFile(string fileName, Simulation game)
    {
      try
      {
        TileMap result = null;
        using (FileStream file = File.Open(fileName, FileMode.Open))
        {
          IFormatter formatter = new BinaryFormatter();
          result = formatter.Deserialize(file) as TileMap;
          file.Close();
        }

        return result;
      }
      catch (System.IO.FileNotFoundException)
      {
        return null;
      }

    }


  }
}
