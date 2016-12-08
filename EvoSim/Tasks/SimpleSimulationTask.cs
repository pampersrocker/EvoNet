using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoSim.Tasks
{
    class SimpleSimulationTask : SimulationTask
    {
        Action<Simulation, GameTime> action;
        public SimpleSimulationTask(Simulation sim, Action<Simulation, GameTime> action) : 
            base(sim)
        {
            this.action = action;
        }

        protected override void Run(GameTime time)
        {
            action?.Invoke(sim, time);
        }
    }
}
