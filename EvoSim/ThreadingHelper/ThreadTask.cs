using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EvoSim.ThreadingHelper
{
    public abstract class ThreadTask
    {
        private bool isDone;
        public bool IsDone
        {
            get { return isDone; }
        }
        protected abstract void Run(GameTime time);
        public WaitCallback Callback;
        public ThreadTask()
        {
            Callback = new WaitCallback(p =>
            {

                DoTask((GameTime)p);
            });
        }

        public void DoTask(GameTime time)
        {
            Run(time);
            isDone = true;
        }

        public void Reset()
        {
            isDone = false;
        }
    }
}
