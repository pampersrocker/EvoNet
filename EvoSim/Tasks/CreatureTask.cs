using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using EvoNet.Objects;

namespace EvoSim.Tasks
{
    class CreatureTask : SimulationArrayTask
    {
        protected int lowerBound;
        protected int upperBound;

        private List<Creature> creaturesToSpawn = new List<Creature>();
        public List<Creature> CreaturesToSpawn
        {
            get { return creaturesToSpawn; }
        }

        public List<Creature> CreaturesToKill
        {
            get
            {
                return creaturesToKill;
            }
        }

        private List<Creature> creaturesToKill = new List<Creature>();

        public void AddCreature(Creature creature)
        {
            creaturesToSpawn.Add(creature);
        }
        public void RemoveCreature(Creature creature)
        {
            creaturesToKill.Add(creature);
        }

        public CreatureTask(Simulation sim, int taskIndex, int numTasks) :
            base(sim, taskIndex, numTasks)
        {

        }

        protected override void Run(GameTime time)
        {
            upperBound = sim.CreatureManager.Creatures.Count * (taskIndex + 1) / numTasks;
            if (upperBound > sim.CreatureManager.Creatures.Count)
            {
                upperBound = sim.CreatureManager.Creatures.Count;
            }
            lowerBound = sim.CreatureManager.Creatures.Count * taskIndex / numTasks;
        }
    }
}
