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
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using EvoSim.ThreadingHelper;
using EvoSim.Tasks;

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
        public List<Creature> Creatures
        {
            get { return creatures; }
        }

        private List<Creature> graveyard = new List<Creature>();
        private List<Creature> CreaturesToKill = new List<Creature>();
        private List<Creature> CreaturesToSpawn = new List<Creature>();

        public List<float> AliveCreaturesRecord = new List<float>();
        public List<float> AverageDeathAgeRecord = new List<float>();

        public Creature OldestCreatureAlive;
        public Creature SelectedCreature;


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
                graveyard.Add(c);
                creatures.Remove(c);
            }
            CreaturesToKill.Clear();
        }

        public override void Initialize(Simulation inGame)
        {
            base.Initialize(inGame);

            GenerateCollisionGrid();

            CreateTasks();
        }

        private void CreateTasks()
        {
            ThreadTaskGroup spawnCreaturesGroup = new ThreadTaskGroup();
            spawnCreaturesGroup.AddTask(new SimpleSimulationTask(simulation,
                (Simulation sim, GameTime time) => 
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
                }));
            //spawnCreaturesGroup.AddDependency(simulation.TileMap.growGroup);
            simulation.TaskManager.AddGroup(spawnCreaturesGroup);

            ThreadTaskGroup readSensorGroup = new ThreadTaskGroup();
            for (int taskIndex = 0; taskIndex < simulation.SimulationConfiguration.NumCreatureTasks; taskIndex++)
            {
                CreatureReadSensorTask task = new CreatureReadSensorTask(simulation, taskIndex, simulation.SimulationConfiguration.NumCreatureTasks);
                readSensorGroup.AddTask(task);
            }
            readSensorGroup.AddDependency(spawnCreaturesGroup);
            simulation.TaskManager.AddGroup(readSensorGroup);
            ThreadTaskGroup actGroup = new ThreadTaskGroup();
            for (int taskIndex = 0; taskIndex < simulation.SimulationConfiguration.NumCreatureTasks; taskIndex++)
            {
                CreatureActTask task = new CreatureActTask(simulation, taskIndex, simulation.SimulationConfiguration.NumCreatureTasks);
                actGroup.AddTask(task);
            }
            actGroup.AddDependency(readSensorGroup);
            simulation.TaskManager.AddGroup(actGroup);

            ThreadTaskGroup mergeAndSpawnGroup = new ThreadTaskGroup();
            MergeCreatureArraysTask mergeTask = new MergeCreatureArraysTask(simulation, actGroup);
            mergeAndSpawnGroup.AddTask(mergeTask);
            mergeAndSpawnGroup.AddDependency(actGroup);
            simulation.TaskManager.AddGroup(mergeAndSpawnGroup);

            ThreadTaskGroup collisionGroup = new ThreadTaskGroup();
            for (int taskIndex = 0; taskIndex < simulation.SimulationConfiguration.NumCreatureTasks; taskIndex++)
            {
                CreatureHandleCollisionTask task = new CreatureHandleCollisionTask(simulation, taskIndex, simulation.SimulationConfiguration.NumCreatureTasks);
                collisionGroup.AddTask(task);
            }
            collisionGroup.AddDependency(mergeAndSpawnGroup);
            simulation.TaskManager.AddGroup(collisionGroup);

            ThreadTaskGroup cleanupAndStatisticsGroup = new ThreadTaskGroup();
            cleanupAndStatisticsGroup.AddDependency(collisionGroup);
            simulation.TaskManager.AddGroup(cleanupAndStatisticsGroup);
            cleanupAndStatisticsGroup.AddTask(new SimpleSimulationTask(simulation, 
                (Simulation sim, GameTime time) => 
                {
                    ResetCollisionGrid();
                }));

            cleanupAndStatisticsGroup.AddTask(new SimpleSimulationTask(simulation,
                (Simulation sim, GameTime time) =>
                {
                    // Collect some statistics
                    year += (float)time.ElapsedGameTime.TotalSeconds;
                    AliveCreaturesRecord.Add(creatures.Count);
                }));
            cleanupAndStatisticsGroup.AddTask(new SimpleSimulationTask(simulation,
                 (Simulation sim, GameTime time) =>
                 {
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
                 }));
        }

        public override bool WantsFastForward
        {
            get
            {
                return true;
            }
        }

        public List<Creature> Graveyard
        {
            get
            {
                return graveyard;
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

        public void Deserialize(string fileName)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(fileName,
                                         FileMode.Open,
                                         FileAccess.Read, FileShare.None);

                creatures = formatter.Deserialize(stream) as List<Creature>;
                
                stream.Close();
                long id = 0;
                foreach (Creature creature in creatures)
                {
                    if (creature.Id > id)
                    {
                        id = creature.Id;
                    }
                    creature.SetupManager(this);
                }
                Creature.currentId = id + 1;

            }
            catch (FileNotFoundException)
            {

            }
        }

        private void SerializeListToFile(string filename, List<Creature> creatures)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(filename,
                                     FileMode.Create,
                                     FileAccess.Write, FileShare.None);

            formatter.Serialize(stream, creatures);
            stream.Close();
        }

        public void Serialize(string filename, string graveYardFilenamePrefix)
        {
            SerializeListToFile(filename, creatures);

            string graveYardFilenameWithDate = string.Format("{0}_{1}.dat", graveYardFilenamePrefix, DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss"));
            string directory =  graveYardFilenameWithDate.Replace(Path.GetFileName(graveYardFilenameWithDate), "");
            Directory.CreateDirectory(directory);
            SerializeListToFile(graveYardFilenameWithDate, graveyard);
            graveyard.Clear();
        }
    }
}
