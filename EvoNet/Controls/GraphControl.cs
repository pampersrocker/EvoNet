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

namespace EvoNet.Controls
{
    public class GraphControl : GraphicsDeviceControl
    {
        public GraphControl()
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
            base.Draw(gameTime);
        }
    }
}
