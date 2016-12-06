using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace EvoSim.Tasks
{
    class CreatureTask : SimulationArrayTask
    {
        protected int lowerBound;
        protected int upperBound;

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
