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

namespace EvoNet.Rendering
{
    class RenderHelper
    {
        private static SpriteBatch spriteBatch = null;
        private static Texture2D whiteTexture = null;
        private static Texture2D whiteCircleTexture = null;

        public static void Ini(SpriteBatch spriteBatch, Texture2D whiteTexture, Texture2D whiteCircleTexture)
        {
            RenderHelper.spriteBatch = spriteBatch;
            RenderHelper.whiteTexture = whiteTexture;
            RenderHelper.whiteCircleTexture = whiteCircleTexture;
        }

        public static void DrawLine(float x0, float y0, float x1, float y1, Color c)
        {
            float xDiff = x0 - x1;
            float yDiff = y0 - y1;
            float angle = (float)Math.Atan2(yDiff, xDiff);
            float dist = (float)Math.Sqrt(xDiff * xDiff + yDiff * yDiff);

            spriteBatch.Draw(whiteTexture, new Rectangle((int)x1, (int)y1, (int)dist, 1), null, c, angle, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
