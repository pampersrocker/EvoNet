using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsGraphicsDevice;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using EvoNet.Input;
using EvoNet.Configuration;
using EvoSim;
using EvoNet.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Color = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Point = System.Drawing.Point;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;


namespace EvoNet.Controls
{
    public class EvoSimControl : GraphicsDeviceControl
    {
        public EvoSimControl()
        {
            MouseWheel += EvoSimControl_MouseWheel;
            MouseEnter += EvoSimControl_MouseEnter;
        }

        private void EvoSimControl_MouseWheel(object sender, MouseEventArgs e)
        {
            Vector2 mousePos = new Vector2(e.X, e.Y);
            inputManager.Zoom(e.Delta / 120.0f, mousePos);
        }

        private void EvoSimControl_MouseEnter(object sender, EventArgs e)
        {
            this.Focus();
        }


        /// <summary>
        /// Default 1x1 white Texture, can be used to draw shapes in any color
        /// </summary>
        public Texture2D WhiteTexture { get; private set; }
        public Texture2D WhiteCircleTexture { get; private set; }

        public SpriteBatch spriteBatch;

        public GameConfig gameConfiguration;
        public InputManager inputManager;


        DateTime lastSerializationTime;

        List<UpdateModule> modules = new List<UpdateModule>();

        public Simulation sim;
        public SimulationRenderer simRenderer;

        private ContentManager contentManager;
        private DateTime lastBackup;

        public ContentManager Content
        {
            get { return contentManager; }
            set { contentManager = value; }
        }
        protected override void Initialize()
        {
            base.Initialize();
            Content = new ContentManager(Services, "Content");

            gameConfiguration = GameConfig.LoadConfigOrDefault();

            // Create default white texture
            WhiteTexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] colorData = new Color[1];
            colorData[0] = Color.White;
            WhiteTexture.SetData(colorData);

            sim = new Simulation();

            modules.Add(sim);

            simRenderer = new SimulationRenderer(sim);


            inputManager = new InputManager();
            inputManager.PositionProvider = this;

            modules.Add(inputManager);

            Fonts.FontArial = Content.Load<SpriteFont>("Arial");
            Fonts.FontArialSmall = Content.Load<SpriteFont>("ArialSmall");

            lastSerializationTime = DateTime.UtcNow;
            lastBackup = lastSerializationTime;
            LoadContent();
        }

        private void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            WhiteCircleTexture = Content.Load<Texture2D>("Map/WhiteCircle512");


            RenderHelper.Ini(WhiteTexture, WhiteCircleTexture);

            sim.Initialize(sim);
            simRenderer.Initialize(Content, GraphicsDevice, spriteBatch);

            inputManager.Initialize(gameConfiguration, sim, simRenderer);

        }

        public void Serialize(bool doBackup, bool waitForCompletion = false)
        {
            sim.TileMap.SerializeToFile("tilemap.dat");
            sim.CreatureManager.Serialize("creatures/creatures", "graveyard/graveyard",doBackup, waitForCompletion);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);


            foreach (var module in modules)
            {
                module.NotifyTick((float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            if (inputManager.EnableFastForward)
            {
                DateTime startFastForward = DateTime.UtcNow;
                double clampedElapsedTime = Math.Min(gameTime.ElapsedGameTime.TotalSeconds, 0.033);
                while ((DateTime.UtcNow - startFastForward).TotalSeconds < clampedElapsedTime)
                {
                    foreach (var module in modules)
                    {
                        if (module.WantsFastForward)
                        {

                            module.NotifyTick((float)gameTime.ElapsedGameTime.TotalSeconds);
                        }
                    }
                }
            }

            // Save progress every 10 seconds
            if (sim.SimulationConfiguration.DoRuntimeSave && 
                (DateTime.UtcNow - lastSerializationTime).TotalSeconds > 10)
            {
                bool doBackup = (DateTime.UtcNow - lastBackup).TotalMinutes > 10;
                if (doBackup)
                {
                    lastBackup = DateTime.UtcNow;
                }
                lastSerializationTime = DateTime.UtcNow;
                Serialize(doBackup);


            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            simRenderer.Draw(gameTime, null);
            base.Draw(gameTime);
        }
    }
}
