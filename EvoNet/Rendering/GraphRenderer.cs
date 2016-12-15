using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet.Rendering
{
    class GraphRenderer
    {

        public static void RenderGraph(SpriteBatch spriteBatch, Rectangle rect, Color barColor, List<float> data, SpriteFont font = null, bool normalized = false)
        {
            if (data.Count == 0)
            {
                return;
            }
            float widthPerBar = (float)rect.Width / data.Count;
            int drawWidth = (int)(widthPerBar < 1 ? 1 : widthPerBar);
            float currentX = 0;
            float maxDataValue = Mathf.Max(data);
            float minDataValue = 0;
            float maxMinDataValue = 0;
            if (normalized)
            {
                minDataValue = Mathf.Min(data);
                maxMinDataValue = maxDataValue - minDataValue;
            }

            //TODO only draw rectangles that are actually seen later. In other words: Do NOT draw more than rect.Width rectangles.
            for(int i = 0; i<data.Count; i++)
            {
                float dataToDraw = data[i];
                float heightPercentage = 0;
                if (normalized)
                {
                    heightPercentage = (dataToDraw - minDataValue) / (maxMinDataValue);
                }
                else
                {
                    heightPercentage = dataToDraw / maxDataValue;
                }
                int height = (int)(rect.Height * heightPercentage);
                //TODO temp rect
                Primitives2D.FillRectangle(spriteBatch, new Rectangle((int)(currentX + rect.X), rect.Y + rect.Height - height, drawWidth, height), barColor);
                currentX += widthPerBar;
            }

            if (font != null)
            {
                spriteBatch.DrawString(font, "" + maxDataValue, new Vector2(rect.X, rect.Y), Color.White);
                spriteBatch.DrawString(font, "" + minDataValue, new Vector2(rect.X, rect.Y + rect.Height - font.MeasureString("" + minDataValue).Y), Color.White);
            }
        }
    }
}
