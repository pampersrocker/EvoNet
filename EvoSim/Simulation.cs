using EvoNet;
using EvoNet.Map;
using EvoNet.Objects;
using EvoNet.ProceduralGeneration;
using EvoSim.Config;
using EvoSim.ThreadingHelper;
using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;

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
            taskManager = new TaskManager(poolSize);
            tileMap = TileMap.DeserializeFromFile("tilemap.dat", this);
            if (tileMap == null)
            {
                tileMap = new TileMap(100, 100, 100.0f);
                tileMap.Initialize(this);

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
            creatureManager.Initialize(this);
            creatureManager.Deserialize("creatures.dat");
        }

        protected override void Update(GameTime deltaTime)
        {

            tileMap.NotifyTick(deltaTime);
            creatureManager.NotifyTick(deltaTime);

            taskManager.ResetTaskGroups(); ;
            taskManager.RunTasks(deltaTime);

        }

        public override void Shutdown()
        {
            base.Shutdown();
            tileMap.Shutdown();
            creatureManager.Shutdown();

            taskManager.Shutdown();
        }
    }
}
