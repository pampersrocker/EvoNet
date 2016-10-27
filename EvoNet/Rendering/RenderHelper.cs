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
        private static Texture2D whiteTexture = null;
        private static Texture2D whiteCircleTexture = null;

        public static void Ini(Texture2D whiteTexture, Texture2D whiteCircleTexture)
        {
            RenderHelper.whiteTexture = whiteTexture;
            RenderHelper.whiteCircleTexture = whiteCircleTexture;
        }

        public static void DrawLine(SpriteBatch spriteBatch, float x0, float y0, float x1, float y1, Color c, int lineWidth = 1)
        {
            Primitives2D.DrawLine(spriteBatch, x0, y0, x1, y1, c);
        }

        public static void DrawCircle(SpriteBatch spriteBatch, float x, float y, float radius, Color c)
        {
            int xD = (int)Math.Round(x - radius);
            int yD = (int)Math.Round(y - radius);
            int radiusD = (int)Math.Round(radius * 2);
            spriteBatch.Draw(whiteCircleTexture, new Rectangle(xD, yD, radiusD, radiusD), c);
        }

    }
}
