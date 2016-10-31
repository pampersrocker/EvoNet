using EvoNet.Configuration;
using EvoNet.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EvoSim
{
    public class Simulation
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

        GameConfig simulationConfiguration;
        public GameConfig SimulationConfiguration
        {
            get { return simulationConfiguration; }
            set { simulationConfiguration = value; }
        }

        public TileMap tileMap;
    }
}
