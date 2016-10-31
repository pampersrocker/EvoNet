using EvoNet.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet.Rendering
{
    class TileMapRenderer : Renderable
    {
        Texture2D Water1Texture { get; set; }
        Texture2D Water2Texture { get; set; }
        Texture2D GrassTexture { get; set; }
        Texture2D SandTexture { get; set; }
        Texture2D BlendMap { get; set; }
        Effect LandShader { get; set; }
        Effect WaterShader { get; set; }
        Rectangle[,] renderRectangles;
        Rectangle[,] rendersourceRectangles;

        SpriteBatch spriteBatch;

        int renderTextureTileFactor;

        TileMap tileMap;
        public EvoNet.Map.TileMap TileMap
        {
            get { return tileMap; }
        }

        public TileMapRenderer(TileMap mapToRender, int inRenderTextureTileFactor = 5)
        {
            tileMap = mapToRender;
            renderTextureTileFactor = inRenderTextureTileFactor;
        }

        public void Draw(GameTime deltaTime, Camera camera)
        {
            // Render land tiles with shader effect to blend between sand and grass
            spriteBatch.Begin(transformMatrix: camera.Matrix, effect: LandShader);
            for (int x = 0; x < tileMap.Width; x++)
            {
                for (int y = 0; y < tileMap.Height; y++)
                {
                    if (tileMap.Types[x, y] == TileType.Land)
                    {
                        Color color = new Color(0.0f, 0.0f, 1.0f, 1 - tileMap.FoodValues[x, y] / 100.0f);
                        spriteBatch.Draw(SandTexture, renderRectangles[x, y], rendersourceRectangles[x, y], color);
                    }
                }
            }
            spriteBatch.End();

            // Render water tiles with animated "water" shader
            spriteBatch.Begin(transformMatrix: camera.Matrix, effect: WaterShader);
            WaterShader.Parameters["Time"].SetValue((float)deltaTime.TotalGameTime.TotalSeconds / 3);
            for (int x = 0; x < tileMap.Width; x++)
            {
                for (int y = 0; y < tileMap.Height; y++)
                {
                    if (tileMap.Types[x, y] == TileType.Water)
                    {
                        spriteBatch.Draw(Water1Texture, renderRectangles[x, y], rendersourceRectangles[x, y], Color.White);
                    }
                }
            }
            spriteBatch.End();
        }

        public void Initialize(ContentManager content, GraphicsDevice graphicsDevice, SpriteBatch inSpriteBatch)
        {
            spriteBatch = inSpriteBatch;

            SandTexture = content.Load<Texture2D>("Map/SandTexture");
            GrassTexture = content.Load<Texture2D>("Map/GrassTexture");
            BlendMap = content.Load<Texture2D>("Map/BlendMap");
            Water1Texture = content.Load<Texture2D>("Map/Water1");
            Water2Texture = content.Load<Texture2D>("Map/Water2");
            BlendMap = content.Load<Texture2D>("Map/BlendMap");
            LandShader = content.Load<Effect>("Map/GrassDisplay");
            WaterShader = content.Load<Effect>("Map/WaterEffect");

            LandShader.Parameters["GrassTexture"].SetValue(GrassTexture);
            LandShader.Parameters["SandTexture"].SetValue(SandTexture);
            LandShader.Parameters["BlendMap"].SetValue(BlendMap);
            WaterShader.Parameters["Water2"].SetValue(Water2Texture);


            renderRectangles = new Rectangle[tileMap.Width, tileMap.Height];
            rendersourceRectangles = new Rectangle[tileMap.Width, tileMap.Height];


            int textureSize = 512; // Assume we have a 512x512 texture
            int sourceSize = textureSize / renderTextureTileFactor;

            for (int x = 0; x < tileMap.Width; x++)
            {
                for (int y = 0; y < tileMap.Height; y++)
                {
                    renderRectangles[x, y] = new Rectangle(
                        (int)(x * tileMap.TileSize),
                        (int)(y * tileMap.TileSize),
                        (int)tileMap.TileSize,
                        (int)tileMap.TileSize);
                    rendersourceRectangles[x, y] = new Rectangle(
                        x * textureSize / renderTextureTileFactor,
                        y * textureSize / renderTextureTileFactor,
                        sourceSize,
                        sourceSize);
                }
            }


        }
    }
}
