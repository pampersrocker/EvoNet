using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using YamlDotNet.Serialization;

namespace EvoNet.Configuration
{
    public class GameConfig
    {
        public List<Keys> MoveUpKeys { get; set; }
        public List<Keys> MoveDownKeys { get; set; }
        public List<Keys> MoveLeftKeys { get; set; }
        public List<Keys> MoveRightKeys { get; set; }

        public float MovementSensitivity { get; set; }

        public float ScaleFactor { get; set; }

        public long GraphCount { get; set; }


        public static GameConfig DefaultConfig
        {
            get
            {
                GameConfig config = new GameConfig();
                config.MoveUpKeys = new List<Keys> { Keys.W, Keys.Up };
                config.MoveDownKeys = new List<Keys> { Keys.S, Keys.Down };
                config.MoveLeftKeys = new List<Keys> { Keys.A, Keys.Left };
                config.MoveRightKeys = new List<Keys> { Keys.D, Keys.Right };
                config.MovementSensitivity = 100.0f;
                config.ScaleFactor = 0.1f;
                config.GraphCount = 10000;
                return config;
            }
        }

        public static GameConfig LoadConfigOrDefault()
        {
            if (File.Exists("Config.cfg"))
            {
                string sourceText = File.ReadAllText("Config.cfg");
                Deserializer yamlDeserializer = new Deserializer();
                GameConfig deserialized = yamlDeserializer.Deserialize<GameConfig>(sourceText);
                CheckLoadedConfig(deserialized);
                SerializerBuilder builder = new SerializerBuilder();
                builder.EmitDefaults();
                Serializer yamlSerializer = builder.Build();
                string serialized = yamlSerializer.Serialize(deserialized);
                File.WriteAllText("Config.cfg", serialized);
                return deserialized;
            }
            else
            {
                // Write out default config if there is no config for easier adjustment
                SerializerBuilder builder = new SerializerBuilder();
                builder.EmitDefaults();
                Serializer yamlSerializer = builder.Build();
                string serialized = yamlSerializer.Serialize(DefaultConfig);
                File.WriteAllText("Config.cfg", serialized);
                return DefaultConfig;
            }
        }

        public static void CheckLoadedConfig(GameConfig config)
        {
            if (config.GraphCount == 0)
            {
                config.GraphCount = 10000;
            }
        }
    }
}
