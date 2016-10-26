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

        public virtual float FixedUpdateTime { get { return 0.016f; } }

        private TimeSpan totalElapsedTime = new TimeSpan();

        public void NotifyTick(GameTime actualTime)
        {
            totalElapsedTime += TimeSpan.FromSeconds(FixedUpdateTime);
            GameTime deltaTime = new GameTime(totalElapsedTime, TimeSpan.FromSeconds(FixedUpdateTime));
            Update(deltaTime);
        }

        public abstract void Update(GameTime deltaTime);
    }
}
