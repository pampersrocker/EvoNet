using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoSim.ThreadingHelper
{
    public class ThreadTaskGroup
    {
        private List<ThreadTask> tasks = new List<ThreadTask>();
        public List<ThreadTask> Tasks
        {
            get { return tasks; }
        }

        public void AddTask(ThreadTask task)
        {
            tasks.Add(task);
        }

        public bool IsDone()
        {
            foreach (ThreadTask task in tasks)
            {
                if (!task.IsDone)
                {
                    return false;
                }
            }
            return true;
        }

        public void Reset()
        {
            foreach (ThreadTask task in tasks)
            {
                task.Reset();
            }
        }

        private List<ThreadTaskGroup> dependencies = new List<ThreadTaskGroup>();

        public void AddDependency(ThreadTaskGroup group)
        {
            dependencies.Add(group);
        }

        public bool CanBeQueued()
        {
            foreach (ThreadTaskGroup dependency in dependencies)
            {
                if (!dependency.IsDone())
                {
                    return false;
                }
            }
            return true;
        }
    }
}
