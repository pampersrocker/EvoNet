using EvoSim.ThreadingHelper;
using System;

namespace EvoSim.Tasks
{
    public abstract class SimulationTask : ThreadTask
    {
        protected Simulation sim;

        public SimulationTask(Simulation sim)
        {
            this.sim = sim;
        }
    }
}
