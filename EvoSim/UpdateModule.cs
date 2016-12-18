using EvoSim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet
{
    [Serializable]
    public abstract class UpdateModule
    {
        public abstract bool WantsFastForward { get; }

        public virtual float FixedUpdateTime
        {
            get
            {
                if (simulation != null)
                {
                    return simulation.SimulationConfiguration.TickInterval;
                }
                else
                {
                    return 0.01f;
                }
            }
        }


        private TimeSpan totalElapsedTime = new TimeSpan();
        public System.TimeSpan TotalElapsedSimulationTime
        {
            get { return totalElapsedTime; }
        }
        [NonSerialized]
        public Simulation simulation;

        public virtual void Initialize(Simulation game)
        {
            this.simulation = game;
        }

        public void NotifyTick(float actualTime)
        {
            totalElapsedTime += TimeSpan.FromSeconds(FixedUpdateTime);
            float deltaTime = FixedUpdateTime;
            Update(deltaTime);
        }

        protected abstract void Update(float deltaTime);

        public virtual void Shutdown() { }
    }
}
