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
using Graph;
using System.Runtime.InteropServices;
using System.Threading;

namespace EvoNet.Controls
{
    struct GraphCache
    {
        public VertexBuffer VertexAreaBuffer;
        public IndexBuffer IndexAreaBuffer;
        public VertexBuffer VertexLineBuffer;
        public int NumElements;
    }

    struct VertexPosition2 : IVertexType
    {
        private static readonly VertexDeclaration InternalVertexDeclaration;

        static VertexPosition2()
        {
            InternalVertexDeclaration = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0)
            );
        }

        public VertexPosition2(Vector2 Position)
        {
            this.Position = Position;
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get
            {
                return InternalVertexDeclaration;
            }
        }

        public Vector2 Position;
    }

    public class GraphControl : GraphicsDeviceControl
    {
        public GraphControl()
        {

        }

        private Dictionary<string, IGraphValueList> graphs = new Dictionary<string, IGraphValueList>();
        private Dictionary<IGraphValueList, GraphCache> graphCaches = new Dictionary<IGraphValueList, GraphCache>();
        public void Add(string name, IGraphValueList list)
        {
            graphs.Add(name, list);
        }

        VertexPositionColor[] Elements = new VertexPositionColor[6];
        Effect Effect;
        protected override void Initialize()
        {
            base.Initialize();
            ContentManager manager = new ContentManager(Services, "Content");
            Effect = manager.Load<Effect>("Color");
        }

        private void DrawCache(IGraphValueList graph, ref GraphCache cache)
        {
            Effect.Parameters["Color"].SetValue(
                new Vector4(
                    graph.Color.R / 255.0f,
                    graph.Color.G / 255.0f,
                    graph.Color.B / 255.0f,
                    (float)graph.PlaneAlpha));
            GraphicsDevice.Indices = cache.IndexAreaBuffer;
            GraphicsDevice.SetVertexBuffer(cache.VertexAreaBuffer);
            foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, cache.VertexAreaBuffer.VertexCount, 0, cache.IndexAreaBuffer.IndexCount / 3);
            }
            Effect.Parameters["Color"].SetValue(
                new Vector4(
                    graph.Color.R / 255.0f,
                    graph.Color.G / 255.0f,
                    graph.Color.B / 255.0f,
                    (float)graph.GraphAlpha));
            GraphicsDevice.SetVertexBuffer(cache.VertexLineBuffer);
            foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, cache.VertexLineBuffer.VertexCount, 0, cache.IndexAreaBuffer.IndexCount / 3);
            }
        }

        private void CreateCache(IGraphValueList graph)
        {
            GraphCache cache;
            int CurrentVertexIndex = 0;
            int graphCount = graph.Count;
            int startIndex = Math.Max(graph.Count - 10000, 0);
            int elementCount = graphCount - startIndex;
            if (elementCount <= 0)
            {
                return;
            }
            Func<IGraphValue, decimal> GetY = (value) => { return value.DisplayValue; };
            Func<IGraphValue, decimal> GetX = (value) => { return value.DisplayPosition; };
            decimal maxY = graph[startIndex].DisplayValue;
            decimal minY = graph[startIndex].DisplayValue;
            decimal maxX = graph[startIndex].DisplayPosition;
            decimal minX = graph[startIndex].DisplayPosition;

            for(int elementIndex = startIndex; elementIndex < graphCount; elementIndex++)
            {
                maxY = Math.Max(maxY, graph[elementIndex].DisplayValue);
                minY = Math.Min(minY, graph[elementIndex].DisplayValue);
                maxX = Math.Max(maxX, graph[elementIndex].DisplayPosition);
                minX = Math.Min(minX, graph[elementIndex].DisplayPosition);
            }

            Func<decimal, float> GetRelativeY = (decimal alpha) =>
            {
                if (minY == maxY)
                {
                    return 0.0f;
                }
                return ((float)((alpha - minY) / (maxY - minY)) - 0.5f) * 2.0f;
            };

            Func<decimal, float> GetRelativeX = (decimal alpha) =>
            {
                return ((float)((alpha - minX) / (maxX - minX)) - 0.5f) * 2.0f;
            };

            VertexPosition2 upperPoint = new VertexPosition2();
            VertexPosition2 lowerPoint = new VertexPosition2();

            upperPoint.Position = new Vector2(-1, GetRelativeY(graph.ElementAt(0).DisplayValue));
            lowerPoint.Position = new Vector2(-1, -1);

            VertexPosition2[] vertexAreaBufferData = new VertexPosition2[((graphCount - startIndex)+1) * 2];
            VertexPosition2[] vertexLineBufferData = new VertexPosition2[((graphCount - startIndex)+1) * 2];
            int[] indexBufferData = new int[((graphCount - startIndex)) * 6];
            int CurrentIndexBufferIndex = 0;

            vertexAreaBufferData[CurrentVertexIndex] = upperPoint;
            vertexAreaBufferData[CurrentVertexIndex + 1] = lowerPoint;

            float lineWidth = (5.0f / Height) / 2.0f;
            Vector2 lineWidthOffset = new Vector2(0.0f, lineWidth / 2.0f);

            vertexLineBufferData[CurrentVertexIndex] = new VertexPosition2(upperPoint.Position + lineWidthOffset);
            vertexLineBufferData[CurrentVertexIndex + 1] = new VertexPosition2(upperPoint.Position - lineWidthOffset);

            CurrentVertexIndex = 2;

            Vector2 lastUpperPoint;

            for (int GraphIndex = startIndex; GraphIndex < graphCount; GraphIndex++)
            {
                float x = GetRelativeX(graph.ElementAt(GraphIndex).DisplayPosition);
                float y = GetRelativeY(graph.ElementAt(GraphIndex).DisplayValue);

                lastUpperPoint = upperPoint.Position;

                upperPoint.Position = new Vector2(x, y);
                lowerPoint.Position = new Vector2(x, -1);
                vertexAreaBufferData[CurrentVertexIndex] = upperPoint;
                vertexAreaBufferData[CurrentVertexIndex + 1] = lowerPoint;

                // Calculate orthogonal vector of direction from last point to this one,
                // so the line keeps its thickness along slopes
                // Without this sloped graphs would get very thin
                Vector2 dirFromLastPoint = upperPoint.Position - lastUpperPoint;
                if (GraphIndex + 1 < graphCount)
                {
                    dirFromLastPoint += new Vector2(
                        GetRelativeX(graph.ElementAt(GraphIndex + 1).DisplayPosition),
                        GetRelativeY(graph.ElementAt(GraphIndex + 1).DisplayValue)) - upperPoint.Position;
                    dirFromLastPoint /= 2;
                }
                dirFromLastPoint.Normalize();
                Vector2 orthogonalDir = new Vector2(-dirFromLastPoint.Y, dirFromLastPoint.X);

                vertexLineBufferData[CurrentVertexIndex] = (new VertexPosition2(upperPoint.Position + orthogonalDir * lineWidth));
                vertexLineBufferData[CurrentVertexIndex + 1] = (new VertexPosition2(upperPoint.Position - orthogonalDir * lineWidth));

                // Counter Clockwise Tris get culled in our current setting
                // So draw 2 triangles in clockwise order
                // -2---+0
                //  |   /|
                //  |  / |
                //  | /  |
                //  |/   |
                // -1---+1
                int LastUpperPoint = CurrentVertexIndex - 2;
                int LastLowerPoint = CurrentVertexIndex - 1;
                int CurrentUpperPoint = CurrentVertexIndex;
                int CurrentLowerPoint = CurrentVertexIndex + 1;
                indexBufferData[CurrentIndexBufferIndex] = LastUpperPoint;
                indexBufferData[CurrentIndexBufferIndex + 1] = CurrentUpperPoint;
                indexBufferData[CurrentIndexBufferIndex + 2] = LastLowerPoint;
                indexBufferData[CurrentIndexBufferIndex + 3] = LastLowerPoint;
                indexBufferData[CurrentIndexBufferIndex + 4] = CurrentUpperPoint;
                indexBufferData[CurrentIndexBufferIndex + 5] = CurrentLowerPoint;

                CurrentIndexBufferIndex += 6;
                CurrentVertexIndex += 2;

            }
            cache.VertexLineBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPosition2), CurrentVertexIndex, BufferUsage.WriteOnly);
            cache.VertexLineBuffer.SetData(vertexLineBufferData);

            cache.VertexAreaBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPosition2), CurrentVertexIndex, BufferUsage.WriteOnly);
            cache.VertexAreaBuffer.SetData(vertexAreaBufferData);
            cache.IndexAreaBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.ThirtyTwoBits, indexBufferData.Length, BufferUsage.WriteOnly);
            cache.IndexAreaBuffer.SetData(indexBufferData);

            cache.NumElements = graphCount;
            graphCaches[graph] = cache;
        }

        private void DrawGraph(IGraphValueList graph)
        {
            if (graph.Count <= 1)
            {
                return;
            }

            GraphCache cache;

            bool needsRedraw = true;
            if (graphCaches.TryGetValue(graph, out cache))
            {
                if (cache.NumElements == graph.Count)
                {
                    needsRedraw = false;
                }
            }


            if (needsRedraw)
            {
                WaitCallback worker = (state) =>
                    {
                        try
                        {
                            CreateCache(graph);
                        }
                        catch (System.Exception ex)
                        {
                        	
                        }
                    };
                ThreadPool.QueueUserWorkItem(worker);
            }
            if (graphCaches.TryGetValue(graph, out cache))
            {
                DrawCache(graph, ref cache);
            }
        }

        protected override void Draw(GameTime gameTime)
        {

            //GraphicsDevice.RasterizerState.MultiSampleAntiAlias = true;
            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            if (pp.MultiSampleCount < 4)
            {
                pp.MultiSampleCount = 4;
                GraphicsDevice.Reset(pp);
            }
            GraphicsDevice.Clear(Color.White);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            foreach (IGraphValueList graph in graphs.Values)
            {
                DrawGraph(graph);
            }

            base.Draw(gameTime);
        }
    }
}
