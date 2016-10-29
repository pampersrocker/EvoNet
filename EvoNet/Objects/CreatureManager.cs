using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using EvoNet.Rendering;
using System.Diagnostics;
using System.Threading;
using EvoNet.ThreadingHelper;
using System.Runtime.CompilerServices;

namespace EvoNet.Objects
{
    public class CreatureManager : UpdateModule
    {
        public static int AmountOfCores = Environment.ProcessorCount;
        public const int COLLISIONGRIDSIZE = 300;
        private static  List<Creature>[,] CollisionGrid = new List<Creature>[COLLISIONGRIDSIZE, COLLISIONGRIDSIZE];

        public static float[] AverageAgeOfLastCreatures = new float[128];
        private int indexForAverageAgeOfLastCreatures = 0;
        private bool AverageAgeOfLastCreaturesAccurate = false;

        private float year = 0;
        private int numberOfDeaths = 0;

        private List<Creature> Creatures = new List<Creature>();
        private List<Creature> CreaturesToKill = new List<Creature>();
        private List<Creature> CreaturesToSpawn = new List<Creature>();

        public List<float> AliveCreaturesRecord = new List<float>();
        public List<float> AverageDeathAgeRecord = new List<float>();

        private Creature OldestCreatureAlive;
        public static Creature SelectedCreature;

        SpriteBatch spriteBatch;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddCreature(Creature c)
        {
            CreaturesToSpawn.Add(c);
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveCreature(Creature c)
        {
            CreaturesToKill.Add(c);
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        private void MergeCreaturesAndSpawnCreatures()
        {
            foreach (Creature c in CreaturesToSpawn)
            {
                Creatures.Add(c);
            }
            CreaturesToSpawn.Clear();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void RemoveCreaturesFromDeathList()
        {
            foreach (Creature c in CreaturesToKill)
            {
                AddDeathAge(c.Age);
                Creatures.Remove(c);
            }
            CreaturesToKill.Clear();
        }

        public override void Initialize(EvoGame inGame)
        {
            base.Initialize(inGame);
            spriteBatch = new SpriteBatch(game.GraphicsDevice);

            GenerateCollisionGrid();
        }

        public override bool WantsFastForward
        {
            get
            {
                return true;
            }
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void AddToCollisionGrid(int x, int y, Creature c)
        {
            CollisionGrid[x, y].Add(c);
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public static List<Creature> GetCollisionGridList(int x, int y)
        {
            return CollisionGrid[x, y];
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void GenerateCollisionGrid()
        {
            for (int i = 0; i < COLLISIONGRIDSIZE; i++)
            {
                for (int k = 0; k < COLLISIONGRIDSIZE; k++)
                {
                    CollisionGrid[i, k] = new List<Creature>();
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void ResetCollisionGrid()
        {
            for(int i = 0; i<COLLISIONGRIDSIZE; i++)
            {
                for(int k = 0; k<COLLISIONGRIDSIZE; k++)
                {
                    CollisionGrid[i, k].Clear();
                }
            }
        }

        private void HandleCollision()
        {
            for(int i = 0; i< AmountOfCores; i++)
            {
                int upperBound = Creatures.Count * (i + 1) / AmountOfCores;
                if (upperBound > Creatures.Count) upperBound = Creatures.Count;
                int lowerBound = Creatures.Count * i / AmountOfCores;
                MultithreadingHelper.StartWork((object state)=>{
                    for (int k = lowerBound; k < upperBound; k++)
                    {
                        Creatures[k].HandleCollisions();
                    }
                    MultithreadingHelper.PulseAndFinish();
                });
            }
            MultithreadingHelper.WaitForEmptyThreadPool();
            ResetCollisionGrid();
        }

        protected override void Update(GameTime deltaTime)
        {
            while (Creatures.Count < 50)
            {
                Creature justSpawned = new Creature(
                    new Vector2(
                        EvoGame.RandomFloat() * game.tileMap.GetWorldWidth(),
                        EvoGame.RandomFloat() * game.tileMap.GetWorldHeight()),
                    EvoGame.RandomFloat() * Mathf.PI * 2);
                justSpawned.Manager = this;
                Creatures.Add(justSpawned);
            }

            for (int i = 0; i < AmountOfCores; i++)
            {
                int upperBound = Creatures.Count * (i + 1) / AmountOfCores;
                if (upperBound > Creatures.Count) upperBound = Creatures.Count;
                int lowerBound = Creatures.Count * i / AmountOfCores;
                MultithreadingHelper.StartWork((object state) => {
                    for (int k = lowerBound; k < upperBound; k++)
                    {
                        Creatures[k].ReadSensors();
                    }
                    MultithreadingHelper.PulseAndFinish();
                });
            }
            MultithreadingHelper.WaitForEmptyThreadPool();
            for (int i = 0; i < AmountOfCores; i++)
            {
                int upperBound = Creatures.Count * (i + 1) / AmountOfCores;
                if (upperBound > Creatures.Count) upperBound = Creatures.Count;
                int lowerBound = Creatures.Count * i / AmountOfCores;
                MultithreadingHelper.StartWork((object state) => {
                    for (int k = lowerBound; k < upperBound; k++)
                    {
                        Creatures[k].Act(deltaTime);
                    }
                    MultithreadingHelper.PulseAndFinish();
                });
            }
            MultithreadingHelper.WaitForEmptyThreadPool();
            numberOfDeaths += CreaturesToKill.Count;

            RemoveCreaturesFromDeathList();
            MergeCreaturesAndSpawnCreatures();
            
            year += (float)deltaTime.ElapsedGameTime.TotalSeconds;

            HandleCollision();

            if (Creatures.Count > 0)
            {

                OldestCreatureAlive = Creatures[0];
                foreach (Creature c in Creatures)
                {
                    if (c.Age > OldestCreatureAlive.Age)
                    {
                        OldestCreatureAlive = c;
                    }
                }
            }

            AliveCreaturesRecord.Add(Creatures.Count);
        }

        public void Draw(GameTime deltaTime)
        {
            foreach (Creature c in Creatures)
            {
                c.Draw();
            }

            DrawGeneralStats();
        }

        private void DrawGeneralStats()
        {
            spriteBatch.Begin();
            Primitives2D.FillRectangle(spriteBatch, new Rectangle(0, 0, 300, 600), AdditionalColors.TRANSPARENTBLACK);
            spriteBatch.DrawString(Fonts.FontArial, "#: " + Creatures.Count, new Vector2(20, 20), Color.Red);
            spriteBatch.DrawString(Fonts.FontArial, "D: " + numberOfDeaths, new Vector2(20, 40), Color.Red);
            spriteBatch.DrawString(Fonts.FontArial, "max(G): " + Creature.maximumGeneration, new Vector2(20, 60), Color.Red);
            spriteBatch.DrawString(Fonts.FontArial, "Y: " + year, new Vector2(20, 80), Color.Red);
            spriteBatch.DrawString(Fonts.FontArial, "LS: " + Creature.oldestCreatureEver.Age + " g: " + Creature.oldestCreatureEver.Generation, new Vector2(20, 100), Color.Red);
            spriteBatch.DrawString(Fonts.FontArial, "LSA: " + OldestCreatureAlive.Age + " g: " + OldestCreatureAlive.Generation, new Vector2(20, 120), Color.Red);
            if (AverageAgeOfLastCreaturesAccurate)
            {
                float averageDeathAge = CalculateAverageAgeOfLastDeadCreatures();
                AverageDeathAgeRecord.Add(averageDeathAge);
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
	            GraphRenderer.RenderGraph(spriteBatch, new Rectangle(20, 200, 260, 100), Color.Blue, AliveCreaturesRecord, Fonts.FontArial, true);
	            spriteBatch.DrawString(Fonts.FontArial, "Average Age on Death Graph ", new Vector2(20, 320), Color.Red);
	            if (AverageAgeOfLastCreaturesAccurate)
	                GraphRenderer.RenderGraph(spriteBatch, new Rectangle(20, 340, 260, 100), Color.Red, AverageDeathAgeRecord, Fonts.FontArial, true);
	            spriteBatch.DrawString(Fonts.FontArial, "Food Available Graph ", new Vector2(20, 460), Color.Red);
	            GraphRenderer.RenderGraph(spriteBatch, new Rectangle(20, 480, 260, 100), Color.Green, EvoGame.Instance.tileMap.FoodRecord, Fonts.FontArial, true);

            }


            if(SelectedCreature != null)
            {
                Primitives2D.FillRectangle(spriteBatch, new Rectangle(800, 0, 500, 450), AdditionalColors.TRANSPARENTBLACK);

                spriteBatch.DrawString(Fonts.FontArial, "Selected Creature: ", new Vector2(820, 50), Color.Red);
                spriteBatch.DrawString(Fonts.FontArial, "A: " + SelectedCreature.Age, new Vector2(820, 70), Color.Red);
                spriteBatch.DrawString(Fonts.FontArial, "E: " + SelectedCreature.Energy, new Vector2(820, 90), Color.Red);
                spriteBatch.DrawString(Fonts.FontArial, "C: " + SelectedCreature.Children.Count, new Vector2(820, 110), Color.Red);
                spriteBatch.DrawString(Fonts.FontArial, "G: " + SelectedCreature.Generation, new Vector2(820, 130), Color.Red);
                spriteBatch.DrawString(Fonts.FontArial, "S: " + (SelectedCreature.Energy > 100 ? "Alive" : "Dead"), new Vector2(820, 150), Color.Red);
                SelectedCreature.DrawCreature(spriteBatch, SelectedCreature.Pos * -1 + new Vector2(1050, 70));
                SelectedCreature.Brain.Draw(spriteBatch, new Rectangle(950, 160, 200, 250));
            }

            spriteBatch.End();
        }

        public void AddDeathAge(float age)
        {
            AverageAgeOfLastCreatures[indexForAverageAgeOfLastCreatures++] = age;
            if(indexForAverageAgeOfLastCreatures >= AverageAgeOfLastCreatures.Length)
            {
                indexForAverageAgeOfLastCreatures = 0;
                AverageAgeOfLastCreaturesAccurate = true;
            }
        }

        public float CalculateAverageAgeOfLastDeadCreatures()
        {
            float ageAverage = 0;
            for(int i = 0; i<AverageAgeOfLastCreatures.Length; i++)
            {
                ageAverage += AverageAgeOfLastCreatures[i];
            }
            return ageAverage / AverageAgeOfLastCreatures.Length;
        }

        public void Serialize(string fileName)
        {
            try
            {
                FileStream file = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write);
                BinaryWriter writer = new BinaryWriter(file);
                writer.Write(Creature.currentId);
                writer.Write(year);
                writer.Write(numberOfDeaths);
                writer.Write(Creature.maximumGeneration);
                writer.Write(Creatures.Count);
                foreach (Creature creature in Creatures)
                {
                    creature.Serialize(writer);
                }
                file.Close();
            }
            catch (System.IO.FileNotFoundException)
            {

            }
        }

        public void Deserialize(string fileName)
        {
            try
            {
                FileStream file = File.Open(fileName, FileMode.Open, FileAccess.Read);
                BinaryReader reader = new BinaryReader(file);
                Creature.currentId = reader.ReadInt64();
                year = reader.ReadSingle();
                numberOfDeaths = reader.ReadInt32();
                Creature.maximumGeneration = reader.ReadInt32();
                int creatureCount = reader.ReadInt32();
                for (int creatureIndex = 0; creatureIndex < creatureCount; creatureIndex++)
                {
                    Creature newCreature = new Creature();
                    newCreature.Manager = this;
                    newCreature.Deserialize(reader);
                    Creatures.Add(newCreature);
                }
                file.Close();
                foreach (Creature creature in Creatures)
                {
                    creature.ConnectAncestry(Creatures);
                }

            }
            catch (System.IO.FileNotFoundException)
            {

            }
        }
    }
}
