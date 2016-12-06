using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace EvoSim.Config
{
    public class SimulationConfiguration
    {
        public float TickInterval { get; set; }
        public int NumThreads { get; set; }
        public int NumCreatureTasks { get; set; }


        public static SimulationConfiguration DefaultConfig
        {
            get
            {
                SimulationConfiguration config = new SimulationConfiguration();
                config.TickInterval = 0.01f;
                config.NumCreatureTasks = 16;
                return config;
            }
        }

        public static SimulationConfiguration LoadConfigOrDefault()
        {
            if (File.Exists("Simulation.cfg"))
            {
                string sourceText = File.ReadAllText("Simulation.cfg");
                Deserializer yamlDeserializer = new Deserializer();
                SimulationConfiguration deserialized = yamlDeserializer.Deserialize<SimulationConfiguration>(sourceText);
                CheckLoadedConfig(deserialized);
                return deserialized;
            }
            else
            {
                // Write out default config if there is no config for easier adjustment
                Serializer yamlSerializer = new Serializer();
                string serialized = yamlSerializer.Serialize(DefaultConfig);
                File.WriteAllText("Simulation.cfg", serialized);
                return DefaultConfig;
            }
        }

        public static void CheckLoadedConfig(SimulationConfiguration config)
        {
            if (config.TickInterval <= 0)
            {
                Console.WriteLine("Reset TickInterval in config from {0} to {1}", config.TickInterval, 0.01f);
                config.TickInterval = 0.01f;
            }
        }
    }
}
