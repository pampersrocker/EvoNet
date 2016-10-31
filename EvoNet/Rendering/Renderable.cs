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
    interface Renderable
    {

        void Initialize(ContentManager content, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch);

        void Draw(GameTime deltaTime, Camera camera);
    }
}
