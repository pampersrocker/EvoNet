using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EvoNet.ThreadingHelper
{
    class MultithreadingHelper
    {
        public static object WorkerLock = new object();
        public static int AmountOfActiveWorkers;

        public static void StartWork(WaitCallback work)
        {
            lock (WorkerLock)
            {
                ++AmountOfActiveWorkers;
            }
            ThreadPool.QueueUserWorkItem(work);
        }

        public static void PulseAndFinish()
        {
            lock (WorkerLock)
            {
                AmountOfActiveWorkers--;
                Monitor.PulseAll(WorkerLock);
            }
        }

        public static void WaitForEmptyThreadPool()
        {
            lock (WorkerLock)
            {
                while(AmountOfActiveWorkers > 0)
                {
                    Monitor.Wait(WorkerLock);
                }
            }
        }
    }
}
