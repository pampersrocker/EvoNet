using EvoSim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EvoCommand
{
    class Program
    {
        static bool keepRunnig = true;

        private static string FormatStatisticsInfo(string stringPrefix, TimeSpan time, Simulation sim, long Iteration, long creaturesUpdateCycles, long graveYardSize, int saveCount)
        {
            string statisticsInfo = string.Format(
                "{0} {1:dd\\d\\ hh\\:mm\\:ss}:\n" +
                "Elapsed Simulation time {2:dd\\d\\ hh\\:mm\\:ss}.\n" +
                "{3:n0} Simulation Iterations ({4:#,000.00}/s).\n" +
                "{5:n0} creature updates ({6:#,000.00}/s).\n" +
                "{7:n0} Creatures lived their live and are put on the graveyard ({8:#,000.00} deaths/s).\n" +
                "Saved {9} times.\n",
                stringPrefix,
                time,
                sim.TotalElapsedSimulationTime,
                Iteration,
                Iteration / time.TotalSeconds,
                creaturesUpdateCycles,
                creaturesUpdateCycles / time.TotalSeconds,
                graveYardSize + sim.CreatureManager.Graveyard.Count,
                (graveYardSize + sim.CreatureManager.Graveyard.Count) / time.TotalSeconds,
                saveCount
            );
            return statisticsInfo;
        }

        static void Main(string[] args)
        {
            Simulation sim = new Simulation();

            sim.Initialize(sim);

            DateTime startTime = DateTime.UtcNow;
            DateTime lastUpdate = DateTime.UtcNow;
            DateTime lastSerializationTime = DateTime.UtcNow;
            DateTime lastConsoleUpdate = DateTime.UtcNow;
            long Iteration = 0;
            TimeSpan elapsedTime = new TimeSpan();
            long creaturesUpdateCycles = 0;
            int saveCount = 0;
            Console.CancelKeyPress += Console_CancelKeyPress;
            Console.CursorVisible = false;
            long graveYardSize = 0;
            // Important if we run this in a actual used command shell (not double clicked on the exe)
            // Otherwise we jump up to the very top of the buffer and start writing there.
            int initialcursorPositionX = Console.CursorLeft;
            int initialcursorPositionY = Console.CursorTop;
            while (keepRunnig)
            {
                DateTime now = DateTime.UtcNow;
                lastUpdate = DateTime.UtcNow;
                elapsedTime = now - startTime;
                creaturesUpdateCycles += sim.CreatureManager.Creatures.Count;
                sim.NotifyTick((float)elapsedTime.TotalSeconds);


                Iteration++;

                // Update console text once per second (console updates are super slow)
                if ((now - lastConsoleUpdate).TotalSeconds >= 1 && keepRunnig && !Console.IsOutputRedirected)
                {
                    lastConsoleUpdate = DateTime.UtcNow;
                    Console.SetCursorPosition(initialcursorPositionX,initialcursorPositionY);
                    string statisticsInfo = FormatStatisticsInfo("Running simulation for", elapsedTime, sim, Iteration, creaturesUpdateCycles, graveYardSize, saveCount);
                    Console.Write(
                        "\r" + 
                        statisticsInfo +
                        "Press Ctrl+C to stop the simulation."
                    );
                }
                // Save progress every 10 seconds
                if ((now - lastSerializationTime).TotalSeconds > 10)
                {
                    lastSerializationTime = DateTime.UtcNow;
                    graveYardSize += sim.CreatureManager.Graveyard.Count;
                    sim.TileMap.SerializeToFile("tilemap.dat");
                    sim.CreatureManager.Serialize("creatures.dat", "graveyard/graveyard");
                    saveCount++;
                }

            }
            Console.WriteLine("Simulation finished, saving....");
            sim.TileMap.SerializeToFile("tilemap.dat");
            sim.CreatureManager.Serialize("creatures.dat", "graveyard/graveyard");
            sim.Shutdown();
            string finalInfo = FormatStatisticsInfo("Ran simulation for", elapsedTime, sim, Iteration, creaturesUpdateCycles, graveYardSize, saveCount);
            Console.WriteLine(finalInfo);
            Console.CursorVisible = true;


        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            keepRunnig = false;
            Console.WriteLine("Cancel Request, waiting for simulation finish");
            Console.CursorVisible = true;
            e.Cancel = true;
        }
    }
}
