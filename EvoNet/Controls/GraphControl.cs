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
using Microsoft.Xna.Framework.Graphics;

using Color = Microsoft.Xna.Framework.Color;
using Microsoft.Xna.Framework.Content;

namespace EvoNet.Controls
{

    public class GraphControl : GraphicsDeviceControl
    {
        public GraphControl()
        {
            
        }
        VertexPositionColor[] Elements = new VertexPositionColor[6];
        Effect Effect;
        protected override void Initialize()
        {
            base.Initialize();
            Elements[0] = new VertexPositionColor(new Vector3(-1,-1,0), Color.Green);
            Elements[1] = new VertexPositionColor(new Vector3(0, 0.5f, 0.0f), Color.Green);
            Elements[2] = new VertexPositionColor(new Vector3(0.5f, 0.5f, 0.0f), Color.Green);
            Elements[3] = new VertexPositionColor(new Vector3(0, 0, 0.0f), Color.Green);
            Elements[4] = new VertexPositionColor(new Vector3(0.5f, 0.5f, 0.0f), Color.Green);
            Elements[5] = new VertexPositionColor(new Vector3(0.5f, 0, 0.0f), Color.Green);
            ContentManager manager = new ContentManager(Services, "Content");
            Effect = manager.Load<Effect>("VertexColor");
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
            foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Elements, 0, 2);
            }
            base.Draw(gameTime);
        }
    }
}
