using System;
using EvoSim;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EvoNet.Rendering
{
    public class SimulationRenderer : Renderable
    {

        Simulation simulation;

        TileMapRenderer tileMapRenderer;
        CreatureRenderer creatureRenderer;

        public SimulationRenderer(Simulation inSimulation)
        {
            simulation = inSimulation;
            cam = new Camera();
        }


        public void Initialize(ContentManager content, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            tileMapRenderer = new TileMapRenderer(simulation.TileMap);

            tileMapRenderer.Initialize(content, graphicsDevice, spriteBatch);
            creatureRenderer = new CreatureRenderer(simulation.CreatureManager);
            creatureRenderer.Initialize(content, graphicsDevice, spriteBatch);

        }

        public void Draw(GameTime deltaTime, Camera camera)
        {
            tileMapRenderer.Draw(deltaTime, cam);
            creatureRenderer.Draw(deltaTime, cam);
        }

        Camera cam;
        public Camera Camera
        {
            get { return cam; }
        }
    }
}
