using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoSim.Tasks
{
    class CreatureActTask : CreatureTask
    {
        public CreatureActTask(Simulation sim, int taskIndex, int numTasks) :
            base(sim, taskIndex, numTasks)
        {

        }

        protected override void Run(float time)
        {
            base.Run(time);
            for (int creatureIndex = lowerBound; creatureIndex < upperBound; creatureIndex++)
            {
                sim.CreatureManager.Creatures[creatureIndex].CurrentTask = this;
                sim.CreatureManager.Creatures[creatureIndex].Act(time);
            }
        }
    }
}
