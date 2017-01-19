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

namespace EvoNet.Controls
{
    public class NetworkRenderControl :
        GraphicsDeviceControl
    {
        public NetworkRenderControl()
        {
        }

        NeuralNetworkRenderer renderer;

        public Simulation Simulation{get;set;}

        private SpriteBatch spriteBatch;

        protected override void Initialize()
        {
            base.Initialize();

            renderer = new NeuralNetworkRenderer();
            spriteBatch = new SpriteBatch(GraphicsDevice);
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
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
            spriteBatch.Begin();
            renderer.Draw(spriteBatch, new Microsoft.Xna.Framework.Rectangle(150, 0, Width-300, Height));
            spriteBatch.End();
        }
    }
}
