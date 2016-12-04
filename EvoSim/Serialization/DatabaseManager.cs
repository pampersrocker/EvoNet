using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvoNet;
using Microsoft.Xna.Framework;
using EvoNet.Objects;
using System.IO;

namespace EvoSim.Serialization
{
    public class DatabaseManager : UpdateModule
    {
        public override bool WantsFastForward
        {
            get
            {
                return false;
            }
        }

        iBoxDB.LocalServer.DB localDatabase;
        iBoxDB.LocalServer.AutoBox localBox;

        public override void Initialize(Simulation game)
        {
            base.Initialize(game);

            iBoxDB.LocalServer.DB.Root(AppDomain.CurrentDomain.BaseDirectory);
            localDatabase = new iBoxDB.LocalServer.DB();
            localDatabase.GetConfig().EnsureTable<Creature>("Creatures", "id");
            //localDatabase.GetConfig().EnsureIndex<Creature>("Creatures", "id");
            localBox = localDatabase.Open();
        }

        public List<Creature> DeserializeCreatures()
        {
            List<Creature> Result = new List<Creature>();

            var Creatures = localBox.Select<Creature>("from Creatures");

            foreach (Creature creature in Creatures)
            {
                creature.SetupManager(simulation.CreatureManager);
                Result.Add(creature);
            }

            return Result;
        }

        protected override void Update(GameTime deltaTime)
        {
            using (var cube = localBox.Cube())
            {
                foreach (Creature creature in simulation.CreatureManager.Creatures)
                {
                    var binding = cube.Bind("Creatures", creature.id);
                    Creature serialized = binding.Select<Creature>();
                    if (serialized != null)
                    {
                        binding.Update(creature);
                    }
                    else
                    {
                        binding.Insert(creature);
                    }
                }
                cube.Commit().Assert("Could not update creatures!");
            }
        }

        public override void Shutdown()
        {
            base.Shutdown();
            localDatabase.Close();
            localBox = null;
        }
    }
}
