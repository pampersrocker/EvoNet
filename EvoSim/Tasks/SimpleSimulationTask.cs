
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoSim.Tasks
{
    class SimpleSimulationTask : SimulationTask
    {
        Action<Simulation, float> action;
        public SimpleSimulationTask(Simulation sim, Action<Simulation, float> action) :
            base(sim)
        {
            this.action = action;
        }

        protected override void Run(float time)
        {
            action?.Invoke(sim, time);
        }
    }
}
