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
using Microsoft.Xna.Framework;
using EvoNet.Rendering;
using EvoNet.Objects;
using EvoSim;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace EvoNet.Controls
{
    public class NetworkRenderControl :
        GraphicsDeviceControl
    {
        public NetworkRenderControl()
        {
        }

        NeuralNetworkRenderer renderer;
        CreatureRenderer creatureRender;

        public Simulation Simulation{get;set;}
        public ContentManager Content { get; private set; }

        private SpriteBatch spriteBatch;

        protected override void Initialize()
        {
            base.Initialize();

            renderer = new NeuralNetworkRenderer();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            renderer.PositionProvider = this;
            Content = new ContentManager(Services, "Content");

        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Simulation != null && Simulation.CreatureManager != null && Simulation.CreatureManager.SelectedCreature != null)
            {
                renderer.Network = Simulation.CreatureManager.SelectedCreature.Brain;

            }
            else
            {
                renderer.Network = null;
            }

            if (creatureRender == null && Simulation != null)
            {
                creatureRender = new CreatureRenderer(Simulation.CreatureManager);
                creatureRender.Initialize(Content, GraphicsDevice, spriteBatch);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
            if (creatureRender != null)
            {
                creatureRender.DrawGeneralStats(new Microsoft.Xna.Framework.Rectangle(0, 0, Width, Height / 4));
            }
            spriteBatch.Begin();
            renderer.Draw(spriteBatch, new Microsoft.Xna.Framework.Rectangle(150, Height/4, Width-400, Height-Height/4));
            spriteBatch.End();
        }
    }
}
