using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    [Serializable]
    public class CreatureManager : UpdateModule
    {
        public static int AmountOfCores = Environment.ProcessorCount;
        public const int COLLISIONGRIDSIZE = 300;
        [NonSerialized]
        private static List<Creature>[,] CollisionGrid = new List<Creature>[COLLISIONGRIDSIZE, COLLISIONGRIDSIZE];

        public static float[] AverageAgeOfLastCreatures = new float[128];
        private int indexForAverageAgeOfLastCreatures = 0;
        public bool AverageAgeOfLastCreaturesAccurate = false;

        public long Tick = 0;
        public int numberOfDeaths = 0;

        private List<Creature> creatures = new List<Creature>();
        public List<Creature> Creatures
        {
            get { return creatures; }
        }

        [NonSerialized]
        private List<Creature> graveyard = new List<Creature>();

        public List<float> AliveCreaturesRecord = new List<float>();
        public List<float> AverageDeathAgeRecord = new List<float>();

        public Creature OldestCreatureAlive;
        public Creature SelectedCreature;

        public override void Initialize(Simulation inGame)
        {
            base.Initialize(inGame);

            if (graveyard == null)
            {
                graveyard = new List<Creature>();
            }

            GenerateCollisionGrid();

            CreateTasks();
        }

        private void CreateTasks()
        {
            ThreadTaskGroup spawnCreaturesGroup = new ThreadTaskGroup();
            spawnCreaturesGroup.AddTask(new SimpleSimulationTask(simulation,
                (Simulation sim, float time) =>
                {
                    while (creatures.Count < sim.SimulationConfiguration.MinCreatures)
                    {
                        Creature justSpawned = new Creature(
                            sim.RandomWorldPosition(),
                            Simulation.RandomFloat() * Mathf.PI * 2,
                            this);
                        lock (this)
                        {
                            creatures.Add(justSpawned);
                            if (SelectedCreature == null || SelectedCreature.Energy <= 100)
                            {
                                SelectedCreature = justSpawned;
                            }
                        }
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
                (Simulation sim, float time) =>
                {
                    ResetCollisionGrid();
                }));

            cleanupAndStatisticsGroup.AddTask(new SimpleSimulationTask(simulation,
                (Simulation sim, float time) =>
                {
                    // Collect some statistics
                    Tick++;
                    AliveCreaturesRecord.Add(creatures.Count);
                }));
            cleanupAndStatisticsGroup.AddTask(new SimpleSimulationTask(simulation,
                 (Simulation sim, float time) =>
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
            for (int i = 0; i < COLLISIONGRIDSIZE; i++)
            {
                for (int k = 0; k < COLLISIONGRIDSIZE; k++)
                {
                    CollisionGrid[i, k].Clear();
                }
            }
        }

        private void HandleCollision()
        {
            for (int i = 0; i < AmountOfCores; i++)
            {
                int upperBound = creatures.Count * (i + 1) / AmountOfCores;
                if (upperBound > creatures.Count) upperBound = creatures.Count;
                int lowerBound = creatures.Count * i / AmountOfCores;
                MultithreadingHelper.StartWork((object state) =>
                {
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

        protected override void Update(float deltaTime)
        {
        }



        public void AddDeathAge(float age)
        {
            AverageAgeOfLastCreatures[indexForAverageAgeOfLastCreatures++] = age;
            if (indexForAverageAgeOfLastCreatures >= AverageAgeOfLastCreatures.Length)
            {
                indexForAverageAgeOfLastCreatures = 0;
                AverageAgeOfLastCreaturesAccurate = true;
            }
        }

        public float CalculateAverageAgeOfLastDeadCreatures()
        {
            float ageAverage = 0;
            for (int i = 0; i < AverageAgeOfLastCreatures.Length; i++)
            {
                ageAverage += AverageAgeOfLastCreatures[i];
            }
            return ageAverage / AverageAgeOfLastCreatures.Length;
        }

        public static CreatureManager Deserialize(string fileName, Simulation sim)
        {
            try
            {
                CreatureManager manager = null;
                IFormatter formatter = new BinaryFormatter();
                using (Stream stream = new FileStream(fileName,
                                         FileMode.Open,
                                         FileAccess.Read, FileShare.None))
                {
                    try
                    {
                        manager = formatter.Deserialize(stream) as CreatureManager;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                };

                if (manager == null)
                {
                    return null;
                }
                manager.Initialize(sim);

                long id = 0;
                foreach (Creature creature in manager.Creatures)
                {
                    if (creature.Id > id)
                    {
                        id = creature.Id;
                    }
                    creature.SetupManager(manager);
                }
                Creature.currentId = id + 1;
                return manager;

            }
            catch (FileNotFoundException)
            {

            }

            return null;
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

        [NonSerialized]
        private object saveLock = new object();

        public void Serialize(string filename, string graveYardFilenamePrefix, bool waitForCompletion = false)
        {
            lock (saveLock)
            {
                List<Creature> asyncGraveYardCopy = graveyard;
                graveyard = new List<Creature>(asyncGraveYardCopy.Count);

                bool done = false;

                WaitCallback worker = (state) =>
                {

                    IFormatter formatter = new BinaryFormatter();
                    string fileNameWithDate = string.Format("{0}_{1}.dat", filename, DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss"));
                    string directory = fileNameWithDate.Replace(Path.GetFileName(fileNameWithDate), "");

                    Directory.CreateDirectory(directory);

                    using (Stream stream = new FileStream(fileNameWithDate,
                                             FileMode.Create,
                                             FileAccess.Write, FileShare.None))
                    {
                        lock (this)
                        {
                            formatter.Serialize(stream, this);
                        }
                    }
                    File.Copy(fileNameWithDate, filename + ".dat");
                    string graveYardFilenameWithDate = string.Format("{0}_{1}.dat", graveYardFilenamePrefix, DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss"));
                    directory = graveYardFilenameWithDate.Replace(Path.GetFileName(graveYardFilenameWithDate), "");
                    Directory.CreateDirectory(directory);
                    SerializeListToFile(graveYardFilenameWithDate, asyncGraveYardCopy);
                    asyncGraveYardCopy.Clear();
                    done = true;
                };
                ThreadPool.QueueUserWorkItem(worker);
                while (waitForCompletion && !done)
                {
                    Thread.Yield();
                }
            }
        }
    }
}
