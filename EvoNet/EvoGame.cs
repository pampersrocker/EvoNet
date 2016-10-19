using EvoNet.Configuration;
using EvoNet.Input;
using EvoNet.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace EvoNet
{
  /// <summary>
  /// This is the main type for your game.
  /// </summary>
  public class EvoGame : Game
  {
    GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;

    TileMap tileMap;
    Camera camera;
    GameConfig gameConfiguration;
    InputManager inputManager;

    /// <summary>
    /// Default 1x1 white Texture, can be used to draw shapes in any color
    /// </summary>
    public static Texture2D WhiteTexture { get; private set; }

    public EvoGame()
    {
      graphics = new GraphicsDeviceManager(this);
      graphics.PreferredBackBufferWidth = 1280;
      graphics.PreferredBackBufferHeight = 720;
      Content.RootDirectory = "Content";
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

      camera = new Camera();

      tileMap = new TileMap(10, 10, 50.0f);
      tileMap.Initialize(this);
      tileMap.Camera = camera;

      inputManager = new InputManager();
      inputManager.Initialize(gameConfiguration, camera);

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

      // Fill the tilemap with some random values
      // TODO: Replace with terrain generation
      Random rand = new Random();
      for (int x = 0; x < tileMap.Width; x++)
      {
        for (int y = 0; y < tileMap.Height; y++)
        {
          tileMap.Types[x, y] = rand.NextDouble() > 0.5 ? TileType.Land : TileType.Water;
        }
      }

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

  
      base.Draw(gameTime);
    }
  }
}
