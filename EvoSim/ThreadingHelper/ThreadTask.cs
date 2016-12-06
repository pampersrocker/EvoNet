using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        protected abstract void Run();

        public void DoTask()
        {
            Run();
            isDone = true;
        }

        public void Reset()
        {
            isDone = false;
        }
    }
}
