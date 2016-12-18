using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EvoSim.Tasks
{
    abstract class SimulationArrayTask : SimulationTask
    {
        protected int taskIndex;
        protected int numTasks;


        public SimulationArrayTask(Simulation sim, int inTaskIndex, int inNumTasks) :
            base(sim)
        {
            taskIndex = inTaskIndex;
            numTasks = inNumTasks;
        }
    }
}
