using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace EvoNet.Objects
{
    public class CreatureManager : UpdateModule
    {
        private float year = 0;
        private int numberOfDeaths = 0;

        public List<Creature> Creatures = new List<Creature>();
        public List<Creature> CreaturesToKill = new List<Creature>();
        public List<Creature> CreaturesToSpawn = new List<Creature>();

        private Creature OldestCreatureAlive;

        EvoGame game;

        SpriteBatch spriteBatch;

        public void Initialize(EvoGame inGame)
        {
            game = inGame;
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
        }

        public override bool WantsFastForward
        {
            get
            {
                return true;
            }
        }

        public override void Update(GameTime deltaTime)
        {
            while (Creatures.Count < 50)
            {
                Creature justSpawned = new Creature(
                    new Vector2(
                        (float)EvoGame.GlobalRandom.NextDouble() * game.tileMap.GetWorldWidth(),
                        (float)EvoGame.GlobalRandom.NextDouble() * game.tileMap.GetWorldHeight()),
                    (float)EvoGame.GlobalRandom.NextDouble() * Mathf.PI * 2);
                justSpawned.Manager = this;
                Creatures.Add(justSpawned);
            }

            foreach (Creature c in Creatures)
            {
                c.ReadSensors();
            }
            foreach (Creature c in Creatures)
            {
                c.Act(deltaTime);
            }
            numberOfDeaths += CreaturesToKill.Count;
            foreach (Creature c in CreaturesToKill)
            {
                Creatures.Remove(c);
            }
            CreaturesToKill.Clear();
            foreach (Creature c in CreaturesToSpawn)
            {
                Creatures.Add(c);
            }
            CreaturesToSpawn.Clear();
            year += (float)deltaTime.ElapsedGameTime.TotalSeconds;

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
        }

        public void Draw(GameTime deltaTime)
        {
            foreach (Creature c in Creatures)
            {
                c.Draw();
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(EvoGame.FontArial, "#: " + Creatures.Count, new Vector2(20, 20), Color.Red);
            spriteBatch.DrawString(EvoGame.FontArial, "Deaths: " + numberOfDeaths, new Vector2(20, 40), Color.Red);
            spriteBatch.DrawString(EvoGame.FontArial, "Maximum Generation: " + Creature.maximumGeneration, new Vector2(20, 60), Color.Red);
            spriteBatch.DrawString(EvoGame.FontArial, "Year: " + year, new Vector2(20, 80), Color.Red);
            spriteBatch.DrawString(EvoGame.FontArial, "Longest Survival: " + Creature.oldestCreatureEver.Age + " g: " + Creature.oldestCreatureEver.Generation, new Vector2(20, 100), Color.Red);

            spriteBatch.DrawString(EvoGame.FontArial, "Longest Survival Alive: " + OldestCreatureAlive.Age + " g: " + OldestCreatureAlive.Generation, new Vector2(20, 120), Color.Red);
            spriteBatch.End();
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
