using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EvoNet.Map
{
    public class TileMap
    {
        float[,] foodValues;
        TileType[,] types;
        Rectangle[,] renderRectangles;
        Rectangle[,] rendersourceRectangles;

        Texture2D Water1Texture { get; set; }
        Texture2D Water2Texture { get; set; }
        Texture2D GrassTexture { get; set; }
        Texture2D SandTexture { get; set; }
        Texture2D BlendMap { get; set; }
        Effect LandShader { get; set; }
        Effect WaterShader { get; set; }

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

        /// <summary>
        /// Creates a new tilemap
        /// </summary>
        /// <param name="width">Number of tiles in horizontal direction</param>
        /// <param name="height">Number of tiles in vertical direction</param>
        /// <param name="inTileSize">Width and Height of a Tile</param>
        /// <param name="renderTextureTileFactor">How many tiles use a single texture until it wraps?
        /// Setting this to a higher value reduces tiled look</param>
        public TileMap(int width, int height, float inTileSize, int renderTextureTileFactor = 5)
        {
            Width = width;
            Height = height;
            foodValues = new float[width, height];
            types = new TileType[width, height];
            renderRectangles = new Rectangle[width, height];
            rendersourceRectangles = new Rectangle[width, height];

            int textureSize = 512; // Assume we have a 512x512 texture
            int sourceSize = textureSize / renderTextureTileFactor;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    renderRectangles[x, y] = new Rectangle(
                        (int)(x * inTileSize),
                        (int)(y * inTileSize),
                        (int)inTileSize,
                        (int)inTileSize);
                    rendersourceRectangles[x, y] = new Rectangle(
                        x * textureSize / renderTextureTileFactor,
                        y * textureSize / renderTextureTileFactor,
                        sourceSize,
                        sourceSize);
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

            SandTexture = game.Content.Load<Texture2D>("Map/SandTexture");
            GrassTexture = game.Content.Load<Texture2D>("Map/GrassTexture");
            BlendMap = game.Content.Load<Texture2D>("Map/BlendMap");
            Water1Texture = game.Content.Load<Texture2D>("Map/Water1");
            Water2Texture = game.Content.Load<Texture2D>("Map/Water2");
            BlendMap = game.Content.Load<Texture2D>("Map/BlendMap");
            LandShader = game.Content.Load<Effect>("Map/GrassDisplay");
            WaterShader = game.Content.Load<Effect>("Map/WaterEffect");

            LandShader.Parameters["GrassTexture"].SetValue(GrassTexture);
            LandShader.Parameters["SandTexture"].SetValue(SandTexture);
            LandShader.Parameters["BlendMap"].SetValue(BlendMap);
            WaterShader.Parameters["Water2"].SetValue(Water2Texture);
        }

        public void Update(GameTime deltaTime)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int k = 0; k < Height; k++)
                {
                    if (IsFertile(i, k))
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
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                return false; //If out of bounds
            }
            if (types[x, y] == TileType.Water)
            {
                return true;
            }
            if (types[x, y] == TileType.Land && foodValues[x, y] > 50)
            {
                return true;
            }
            return false;
        }

        public bool IsFertile(int x, int y)
        {
            if (types[x, y] == TileType.Land)
            {
                if (foodValues[x, y] > 50)
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
            Matrix? UsedMatrix = null;
            if (Camera != null)
            {
                UsedMatrix = Camera.Matrix;
            }

            // Render land tiles with shader effect to blend between sand and grass
            spriteBatch.Begin(transformMatrix: UsedMatrix, effect: LandShader);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (types[x, y] == TileType.Land)
                    {
                        Color color = new Color(0.0f, 0.0f, 1.0f, 1 - foodValues[x, y] / 100.0f);
                        spriteBatch.Draw(SandTexture, renderRectangles[x, y], rendersourceRectangles[x, y], color);
                    }
                }
            }
            spriteBatch.End();

            // Render water tiles with animated "water" shader
            spriteBatch.Begin(transformMatrix: UsedMatrix, effect: WaterShader);
            WaterShader.Parameters["Time"].SetValue((float)deltaTime.TotalGameTime.TotalSeconds / 3);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (types[x, y] == TileType.Water)
                    {
                        spriteBatch.Draw(Water1Texture, renderRectangles[x, y], rendersourceRectangles[x, y], Color.White);
                    }
                }
            }
            spriteBatch.End();

        }


    }
}
