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
        public int MinCreatures { get; set; }
        public int TileMapSizeX { get; set; }
        public int TileMapSizeY { get; set; }
        public int NumberOfStartNeuronLayers { get; set; }
        public float AddRemoveLayerPercentage { get; set; }
        public bool UseMate { get; set; }
        public float MateBrainPercentage { get; set; }


        public static SimulationConfiguration DefaultConfig
        {
            get
            {
                SimulationConfiguration config = new SimulationConfiguration();
                config.TickInterval = 0.01f;
                config.NumCreatureTasks = 8;
                config.MinCreatures = 50;
                config.TileMapSizeX = 100;
                config.TileMapSizeY = 100;
                config.NumberOfStartNeuronLayers = 1;
                config.UseMate = true;
                config.MateBrainPercentage = 0.5f;
                config.AddRemoveLayerPercentage = 0.05f;
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
                // Write config with updated values
                Serializer yamlSerializer = new Serializer();
                string serialized = yamlSerializer.Serialize(deserialized);
                File.WriteAllText("Simulation.cfg", serialized);
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

            if (config.NumCreatureTasks == 0)
            {
                config.NumCreatureTasks = Environment.ProcessorCount;
            }
            config.NumberOfStartNeuronLayers = Math.Max(1, config.NumberOfStartNeuronLayers);
            config.MinCreatures = Math.Max(1, config.MinCreatures);
            config.TileMapSizeX = Math.Max(1, config.TileMapSizeX);
            config.TileMapSizeY = Math.Max(1, config.TileMapSizeY);
            config.MateBrainPercentage = Math.Max(0.0f, Math.Min(1.0f, config.MateBrainPercentage));
            config.AddRemoveLayerPercentage = Math.Max(0.0f, Math.Min(1.0f, config.AddRemoveLayerPercentage));
        }
    }
}
