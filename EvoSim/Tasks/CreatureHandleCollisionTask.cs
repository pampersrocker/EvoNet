using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoSim.Tasks
{
    class CreatureHandleCollisionTask : CreatureTask
    {
        public CreatureHandleCollisionTask(Simulation sim, int taskIndex, int numTasks) :
            base(sim, taskIndex, numTasks)
        {

        }

        protected override void Run(float time)
        {
            base.Run(time);
            for (int creatureIndex = lowerBound; creatureIndex < upperBound; creatureIndex++)
            {
                sim.CreatureManager.Creatures[creatureIndex].HandleCollisions();
            }
        }
    }
}
