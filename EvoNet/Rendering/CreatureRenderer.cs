using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using EvoNet.Objects;

namespace EvoNet.Rendering
{
    class CreatureRenderer : Renderable
    {
        CreatureManager manager;

        SpriteBatch spriteBatch;

        Creature selectedCreature;

        NeuralNetworkRenderer networkRenderer;

        private static Texture2D bodyTex = null;
        private static Texture2D feelerTex = null;

        public CreatureRenderer(CreatureManager inManager)
        {
            manager = inManager;
        }

        public void Initialize(ContentManager content, GraphicsDevice graphicsDevice, SpriteBatch inSpriteBatch)
        {
            spriteBatch = inSpriteBatch;

            bodyTex = content.Load<Texture2D>("Map/WhiteCircle512");
            feelerTex = content.Load<Texture2D>("Map/WhiteCircle512");

            networkRenderer = new NeuralNetworkRenderer();
        }

        public void Draw(GameTime deltaTime, Camera camera)
        {
            spriteBatch.Begin(transformMatrix: camera.Matrix);
            foreach (Creature c in manager.Creatures)
            {
                DrawCreature(c);
            }
            spriteBatch.End();

            DrawGeneralStats();
        }

        private void DrawGeneralStats()
        {
            spriteBatch.Begin();
            Primitives2D.FillRectangle(spriteBatch, new Rectangle(0, 0, 300, 600), AdditionalColors.TRANSPARENTBLACK);
            spriteBatch.DrawString(Fonts.FontArial, "#: " + manager.Creatures.Count, new Vector2(20, 20), Color.Red);
            spriteBatch.DrawString(Fonts.FontArial, "D: " + manager.numberOfDeaths, new Vector2(20, 40), Color.Red);
            spriteBatch.DrawString(Fonts.FontArial, "max(G): " + Creature.maximumGeneration, new Vector2(20, 60), Color.Red);
            spriteBatch.DrawString(Fonts.FontArial, "Y: " + manager.year, new Vector2(20, 80), Color.Red);
            spriteBatch.DrawString(Fonts.FontArial, "LS: " + Creature.oldestCreatureEver.Age + " g: " + Creature.oldestCreatureEver.Generation, new Vector2(20, 100), Color.Red);
            spriteBatch.DrawString(Fonts.FontArial, "LSA: " + manager.OldestCreatureAlive.Age + " g: " + manager.OldestCreatureAlive.Generation, new Vector2(20, 120), Color.Red);
            if (manager.AverageAgeOfLastCreaturesAccurate)
            {
                float averageDeathAge = manager.CalculateAverageAgeOfLastDeadCreatures();
                manager.AverageDeathAgeRecord.Add(averageDeathAge);
                spriteBatch.DrawString(Fonts.FontArial, "AvgDA: " + averageDeathAge, new Vector2(20, 140), Color.Red);
            }

            if (EvoGame.Instance.inputManager.EnableFastForward)
            {
                spriteBatch.DrawString(Fonts.FontArial, "Graph rendering disabled", new Vector2(20, 180), Color.Red);
                spriteBatch.DrawString(Fonts.FontArial, "during fast forward!", new Vector2(20, 200), Color.Red);
            }
            else
            {
                spriteBatch.DrawString(Fonts.FontArial, "Creatures Alive Graph ", new Vector2(20, 180), Color.Red);
                GraphRenderer.RenderGraph(spriteBatch, new Rectangle(20, 200, 260, 100), Color.Blue, manager.AliveCreaturesRecord, Fonts.FontArial, true);
                spriteBatch.DrawString(Fonts.FontArial, "Average Age on Death Graph ", new Vector2(20, 320), Color.Red);
                if (manager.AverageAgeOfLastCreaturesAccurate)
                    GraphRenderer.RenderGraph(spriteBatch, new Rectangle(20, 340, 260, 100), Color.Red, manager.AverageDeathAgeRecord, Fonts.FontArial, true);
                spriteBatch.DrawString(Fonts.FontArial, "Food Available Graph ", new Vector2(20, 460), Color.Red);
                GraphRenderer.RenderGraph(spriteBatch, new Rectangle(20, 480, 260, 100), Color.Green, manager.simulation.TileMap.FoodRecord, Fonts.FontArial, true);

            }

            selectedCreature = manager.SelectedCreature;

            if(selectedCreature != null)
            {
                Primitives2D.FillRectangle(spriteBatch, new Rectangle(800, 0, 500, 450), AdditionalColors.TRANSPARENTBLACK);

                spriteBatch.DrawString(Fonts.FontArial, "Selected Creature: ", new Vector2(820, 50), Color.Red);
                spriteBatch.DrawString(Fonts.FontArial, "A: " + selectedCreature.Age, new Vector2(820, 70), Color.Red);
                spriteBatch.DrawString(Fonts.FontArial, "E: " + selectedCreature.Energy, new Vector2(820, 90), Color.Red);
                spriteBatch.DrawString(Fonts.FontArial, "C: " + selectedCreature.Children.Count, new Vector2(820, 110), Color.Red);
                spriteBatch.DrawString(Fonts.FontArial, "G: " + selectedCreature.Generation, new Vector2(820, 130), Color.Red);
                spriteBatch.DrawString(Fonts.FontArial, "S: " + (selectedCreature.Energy > 100 ? "Alive" : "Dead"), new Vector2(820, 150), Color.Red);
                DrawCreature(selectedCreature, selectedCreature.Pos * -1 + new Vector2(1050, 70));
                networkRenderer.Network = selectedCreature.Brain;
                networkRenderer.Draw(spriteBatch, new Rectangle(950, 160, 200, 250));
            }

            spriteBatch.End();
        }

        private void DrawCreature(Creature c, Vector2 offset = new Vector2())
        {
            spriteBatch.DrawLine(c.Pos + offset, c.FeelerPos +offset, Color.White);
            spriteBatch.Draw(bodyTex, new Rectangle((int)(c.Pos.X + offset.X - Creature.CREATURESIZE / 2), (int)(c.Pos.Y + offset.Y - Creature.CREATURESIZE / 2), Creature.CREATURESIZE, Creature.CREATURESIZE), c.Color_inv);
            spriteBatch.Draw(bodyTex, new Rectangle((int)(c.Pos.X + offset.X - (Creature.CREATURESIZE - 4) / 2), (int)(c.Pos.Y + offset.Y - (Creature.CREATURESIZE - 4) / 2), Creature.CREATURESIZE - 4, Creature.CREATURESIZE - 4), c.Color);
            spriteBatch.Draw(feelerTex, new Rectangle((int)(c.FeelerPos.X + offset.X - 5), (int)(c.FeelerPos.Y + offset.Y - 5), 10, 10), c.TimeSinceLastAttack > Creature.TIMEBETWEENATTACKS ? Color.Blue : Color.Red);
        }

    }
}
