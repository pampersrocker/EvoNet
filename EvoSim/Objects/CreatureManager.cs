using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Diagnostics;
using System.Threading;
using EvoNet.ThreadingHelper;
using System.Runtime.CompilerServices;
using EvoSim;

namespace EvoNet.Objects
{
    public class CreatureManager : UpdateModule
    {
        public static int AmountOfCores = Environment.ProcessorCount;
        public const int COLLISIONGRIDSIZE = 300;
        private static  List<Creature>[,] CollisionGrid = new List<Creature>[COLLISIONGRIDSIZE, COLLISIONGRIDSIZE];

        public static float[] AverageAgeOfLastCreatures = new float[128];
        private int indexForAverageAgeOfLastCreatures = 0;
        public bool AverageAgeOfLastCreaturesAccurate = false;

        public float year = 0;
        public int numberOfDeaths = 0;

        private List<Creature> creatures = new List<Creature>();
        public System.Collections.Generic.List<EvoNet.Objects.Creature> Creatures
        {
            get { return creatures; }
        }
        private List<Creature> CreaturesToKill = new List<Creature>();
        private List<Creature> CreaturesToSpawn = new List<Creature>();

        public List<float> AliveCreaturesRecord = new List<float>();
        public List<float> AverageDeathAgeRecord = new List<float>();

        public Creature OldestCreatureAlive;
        public Creature SelectedCreature;


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
                creatures.Add(c);
            }
            CreaturesToSpawn.Clear();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void RemoveCreaturesFromDeathList()
        {
            foreach (Creature c in CreaturesToKill)
            {
                AddDeathAge(c.Age);
                creatures.Remove(c);
            }
            CreaturesToKill.Clear();
        }

        public override void Initialize(Simulation inGame)
        {
            base.Initialize(inGame);

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
                int upperBound = creatures.Count * (i + 1) / AmountOfCores;
                if (upperBound > creatures.Count) upperBound = creatures.Count;
                int lowerBound = creatures.Count * i / AmountOfCores;
                MultithreadingHelper.StartWork((object state)=>{
                    for (int k = lowerBound; k < upperBound; k++)
                    {
                        creatures[k].HandleCollisions();
                    }
                    MultithreadingHelper.PulseAndFinish();
                });
            }
            MultithreadingHelper.WaitForEmptyThreadPool();
            ResetCollisionGrid();
        }

        protected override void Update(GameTime deltaTime)
        {
            while (creatures.Count < 50)
            {
                Creature justSpawned = new Creature(
                    new Vector2(
                        Simulation.RandomFloat() * simulation.TileMap.GetWorldWidth(),
                        Simulation.RandomFloat() * simulation.TileMap.GetWorldHeight()),
                    Simulation.RandomFloat() * Mathf.PI * 2,
                    this);
                creatures.Add(justSpawned);
            }

            for (int i = 0; i < AmountOfCores; i++)
            {
                int upperBound = creatures.Count * (i + 1) / AmountOfCores;
                if (upperBound > creatures.Count) upperBound = creatures.Count;
                int lowerBound = creatures.Count * i / AmountOfCores;
                MultithreadingHelper.StartWork((object state) => {
                    for (int k = lowerBound; k < upperBound; k++)
                    {
                        creatures[k].ReadSensors();
                    }
                    MultithreadingHelper.PulseAndFinish();
                });
            }
            MultithreadingHelper.WaitForEmptyThreadPool();
            for (int i = 0; i < AmountOfCores; i++)
            {
                int upperBound = creatures.Count * (i + 1) / AmountOfCores;
                if (upperBound > creatures.Count) upperBound = creatures.Count;
                int lowerBound = creatures.Count * i / AmountOfCores;
                MultithreadingHelper.StartWork((object state) => {
                    for (int k = lowerBound; k < upperBound; k++)
                    {
                        creatures[k].Act(deltaTime);
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

            if (creatures.Count > 0)
            {

                OldestCreatureAlive = creatures[0];
                foreach (Creature c in creatures)
                {
                    if (c.Age > OldestCreatureAlive.Age)
                    {
                        OldestCreatureAlive = c;
                    }
                }
            }

            AliveCreaturesRecord.Add(creatures.Count);
        }

        public void Draw(GameTime deltaTime)
        {

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
                writer.Write(creatures.Count);
                foreach (Creature creature in creatures)
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
                    Creature newCreature = new Creature(this);
                    newCreature.Deserialize(reader);
                    creatures.Add(newCreature);
                }
                file.Close();
                foreach (Creature creature in creatures)
                {
                    creature.ConnectAncestry(creatures);
                }

            }
            catch (System.IO.FileNotFoundException)
            {

            }
        }
    }
}
