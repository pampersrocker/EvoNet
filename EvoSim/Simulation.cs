using EvoNet;
using EvoNet.Map;
using EvoNet.Objects;
using EvoNet.ProceduralGeneration;
using EvoSim.Config;
using EvoSim.ThreadingHelper;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace EvoSim
{
    public class Simulation : UpdateModule
    {

        private static Random GlobalRandom = new Random();
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static float RandomFloat()
        {
            return (float)GlobalRandom.NextDouble();
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Vector2 RandomWorldPosition()
        {
            return new Vector2(Simulation.RandomFloat() * TileMap.GetWorldWidth(),
                               Simulation.RandomFloat() * TileMap.GetWorldHeight());
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static int RandomInt(int min, int max)
        {
            return GlobalRandom.Next(min, max);
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static int RandomInt(int max)
        {
            return GlobalRandom.Next(max);
        }

        TaskManager taskManager;
        public TaskManager TaskManager
        {
            get { return taskManager; }
        }
        SimulationConfiguration simulationConfiguration;
        public SimulationConfiguration SimulationConfiguration
        {
            get { return simulationConfiguration; }
            set { simulationConfiguration = value; }
        }

        public override bool WantsFastForward
        {
            get
            {
                return true;
            }
        }

        private TileMap tileMap;
        public TileMap TileMap
        {
            get { return tileMap; }
        }

        private CreatureManager creatureManager = new CreatureManager();

        public CreatureManager CreatureManager
        {
            get { return creatureManager; }
            set { creatureManager = value; }
        }
        public override void Initialize(Simulation simulation)
        {
            base.Initialize(simulation);

            simulationConfiguration = SimulationConfiguration.LoadConfigOrDefault();
            int poolSize = simulationConfiguration.NumThreads;
            if (poolSize == 0)
            {
                poolSize = Environment.ProcessorCount;
            }
            poolSize = Math.Max(1, poolSize);
            //ThreadPool.SetMaxThreads(poolSize, poolSize);
            taskManager = new TaskManager();
            tileMap = TileMap.DeserializeFromFile("tilemap.dat", this);
            bool validTileMap = tileMap != null &&
              tileMap.Width == simulationConfiguration.TileMapSizeX &&
              tileMap.Height == simulationConfiguration.TileMapSizeY;
            if (!validTileMap)
            {
                tileMap = new TileMap(
                  simulation.SimulationConfiguration.TileMapSizeX,
                  simulation.simulationConfiguration.TileMapSizeY,
                  100.0f
                );

                ValueNoise2D vn = new ValueNoise2D(tileMap.Width, tileMap.Height);
                vn.startFrequencyX = 10;
                vn.startFrequencyY = 10;
                vn.calculate();
                float[,] heightMap = vn.getHeightMap();
                for (int x = 0; x < tileMap.Width; x++)
                {
                    for (int y = 0; y < tileMap.Height; y++)
                    {
                        tileMap.SetTileType(x, y, heightMap[x, y] > 0.5 ? TileType.Land : TileType.Water);
                    }
                }

                tileMap.SerializeToFile("tilemap.dat");


            }
            tileMap.Initialize(this);

            CreatureManager deserialized = CreatureManager.Deserialize("creatures/creatures.dat", this);
            if (deserialized != null)
            {
                creatureManager = deserialized;
            }
            else
            {
                creatureManager.Initialize(this);
            }
        }

        protected override void Update(float deltaTime)
        {

            tileMap.NotifyTick(deltaTime);
            creatureManager.NotifyTick(deltaTime);

            taskManager.ResetTaskGroups();
            taskManager.RunTasks(deltaTime);

        }

        public override void Shutdown()
        {
            base.Shutdown();
            tileMap.Shutdown();
            creatureManager.Shutdown();

            //taskManager.Shutdown();
        }
    }
}
