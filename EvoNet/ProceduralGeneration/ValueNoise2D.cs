using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet.ProceduralGeneration
{
    class ValueNoise2D
    {
        public int octaves = 8;
        public int startFrequencyX = 2;
        public int startFrequencyY = 2;
        public readonly int WIDTH;
        public readonly int HEIGHT;

        private float[,] heightMap;


        public ValueNoise2D(int width, int height)
        {
            WIDTH = width;
            HEIGHT = height;

            heightMap = new float[width, height];
        }

        public void calculate()
        {
            int currentFrequencyX = startFrequencyX;
            int currentFrequencyY = startFrequencyY;

            float currentAlpha = 1;

            for(int oc = 0; oc< octaves; oc++)
            {
                if(oc > 0)
                {
                    currentFrequencyX *= 2;
                    currentFrequencyY *= 2;
                    currentAlpha /= 2;
                }

                float[,] discretePoints = new float[currentFrequencyX+1, currentFrequencyY+1];
                for(int i = 0; i<currentFrequencyX + 1; i++)
                {
                    for(int k = 0; k<currentFrequencyY + 1; k++)
                    {
                        discretePoints[i, k] = (float)EvoGame.GlobalRandom.NextDouble() * currentAlpha;
                    }
                }

                for(int i = 0; i< WIDTH; i++)
                {
                    for(int k = 0; k<HEIGHT; k++)
                    {
                        float currentX = i / (float)WIDTH * currentFrequencyX;
                        float currentY = k / (float)HEIGHT * currentFrequencyY;

                        int indexX = (int)currentX;
                        int indexY = (int)currentY;

                        float w0 = interpolate(discretePoints[indexX, indexY], discretePoints[indexX + 1, indexY], currentX - indexX);
                        float w1 = interpolate(discretePoints[indexX, indexY + 1], discretePoints[indexX + 1, indexY + 1], currentX - indexX);
                        float w = interpolate(w0, w1, currentY - indexY);

                        heightMap[i, k] += w;
                    }
                }
            }

            normalize();
        }

        private void normalize()
        {
            float min = float.MaxValue;
            for(int i = 0; i< WIDTH; i++)
            {
                for(int k = 0; k< HEIGHT; k++)
                {
                    if(heightMap[i,k] < min)
                    {
                        min = heightMap[i, k];
                    }
                }
            }

            for(int i = 0; i< WIDTH; i++)
            {
                for(int k = 0; k< HEIGHT; k++)
                {
                    heightMap[i, k] -= min;
                }
            }

            float max = float.MinValue;
            for (int i = 0; i < WIDTH; i++)
            {
                for (int k = 0; k < HEIGHT; k++)
                {
                    if (heightMap[i, k] > max)
                    {
                        max = heightMap[i, k];
                    }
                }
            }

            for (int i = 0; i < WIDTH; i++)
            {
                for (int k = 0; k < HEIGHT; k++)
                {
                    heightMap[i, k] /= max;
                }
            }
        }

        private float interpolate(float a, float b, float t)
        {
            return Mathf.InterpolateCosine(a, b, t);
        }

        public float[,] getHeightMap()
        {
            return heightMap;
        }
    }
}
