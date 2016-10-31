using EvoSim;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet
{
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

        public Simulation simulation;

        public virtual void Initialize(Simulation game)
        {
            this.simulation = game;
        }

        public void NotifyTick(GameTime actualTime)
        {
            totalElapsedTime += TimeSpan.FromSeconds(FixedUpdateTime);
            GameTime deltaTime = new GameTime(totalElapsedTime, TimeSpan.FromSeconds(FixedUpdateTime));
            Update(deltaTime);
        }

        protected abstract void Update(GameTime deltaTime);
    }
}
