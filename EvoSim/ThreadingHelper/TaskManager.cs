
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EvoSim.ThreadingHelper
{
    public class TaskManager
    {

        private List<ThreadTaskGroup> pendingTaskGroups = new List<ThreadTaskGroup>();
        private List<ThreadTaskGroup> completedTaskGroups = new List<ThreadTaskGroup>();
        private List<ThreadTaskGroup> runningTaskGroups = new List<ThreadTaskGroup>();
        private float currentTime;

        public TaskManager()
        {
        }

        public void AddGroup(ThreadTaskGroup group)
        {
            pendingTaskGroups.Add(group);
        }

        public void RunTasks(float time)
        {
            currentTime = time;
            while (pendingTaskGroups.Count > 0 || runningTaskGroups.Count > 0)
            {
                for (int groupIndex = pendingTaskGroups.Count - 1; groupIndex >= 0; --groupIndex)
                {
                    if (pendingTaskGroups[groupIndex].CanBeQueued())
                    {
                        ThreadTaskGroup taskGroup = pendingTaskGroups[groupIndex];
                        runningTaskGroups.Add(taskGroup);
                        pendingTaskGroups.RemoveAt(groupIndex);
                        foreach (ThreadTask task in taskGroup.Tasks)
                        {
                            ThreadPool.QueueUserWorkItem(task.Callback, time);
                        }
                        Thread.Yield();

                    }
                }
                for (int runningGroupIndex = runningTaskGroups.Count - 1; runningGroupIndex >= 0; --runningGroupIndex)
                {
                    if (runningTaskGroups[runningGroupIndex].IsDone())
                    {
                        completedTaskGroups.Add(runningTaskGroups[runningGroupIndex]);
                        runningTaskGroups.RemoveAt(runningGroupIndex);
                    }
                }
                Thread.Yield();
            }
        }

        public void ResetTaskGroups()
        {
            if (runningTaskGroups.Count > 0)
            {
                throw new InvalidOperationException("Cannot reset task groups while we have running tasks.");
            }
            foreach (ThreadTaskGroup group in completedTaskGroups)
            {
                group.Reset();
            }
            pendingTaskGroups.AddRange(completedTaskGroups);
            completedTaskGroups.Clear();
        }
    }
}
