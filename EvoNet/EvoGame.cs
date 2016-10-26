using EvoNet.Configuration;
using EvoNet.Input;
using EvoNet.Map;
using EvoNet.ProceduralGeneration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using EvoNet.Objects;

namespace EvoNet
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class EvoGame : Game
    {
        
        public static Random GlobalRandom = new Random();
        public static EvoGame Instance;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public TileMap tileMap;
        GameConfig gameConfiguration;
        InputManager inputManager;

        CreatureManager creatureManager = new CreatureManager();

        public static SpriteFont FontArial { get; set; }

        DateTime lastSerializationTime;

        /// <summary>
        /// Default 1x1 white Texture, can be used to draw shapes in any color
        /// </summary>
        public static Texture2D WhiteTexture { get; private set; }

        public EvoGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
            Instance = this;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            gameConfiguration = GameConfig.LoadConfigOrDefault();

            // Create default white texture
            WhiteTexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] colorData = new Color[1];
            colorData[0] = Color.White;
            WhiteTexture.SetData(colorData);

            Creature.Initialize();

            inputManager = new InputManager();
            inputManager.Initialize(gameConfiguration, Camera.instanceGameWorld);

            FontArial = Content.Load<SpriteFont>("Arial");

            lastSerializationTime = DateTime.UtcNow;


            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            tileMap = TileMap.DeserializeFromFile("tilemap.dat", this);
            if (tileMap == null)
            {
                tileMap = new TileMap(100, 100, 100.0f);
                tileMap.Initialize(this);

                ValueNoise2D vn = new ValueNoise2D(tileMap.Width, tileMap.Height);
                vn.startFrequencyX = 10;
                vn.startFrequencyY = 10;
                vn.calculate();
                float[,] heightMap = vn.getHeightMap();
                for (int x = 0; x < tileMap.Width; x++)
                {
                    for (int y = 0; y < tileMap.Height; y++)
                    {
                        tileMap.SetTileType(x, y, heightMap[x, y] > 0.5 ? TileType.Land : TileType.Water);
                    }
                }

                tileMap.SerializeToFile("tilemap.dat");
            }

            creatureManager.Initialize(this);
            creatureManager.Deserialize("creatures.dat");


            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            inputManager.Update(gameTime);

            tileMap.Update(gameTime);

            creatureManager.Update(gameTime);

            // Save progress every minute
            if ((DateTime.UtcNow - lastSerializationTime).TotalSeconds > 10)
            {
                lastSerializationTime = DateTime.UtcNow;
                tileMap.SerializeToFile("tilemap.dat");
                creatureManager.Serialize("creatures.dat");
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            tileMap.Draw(gameTime);

            creatureManager.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
