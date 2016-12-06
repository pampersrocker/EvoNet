using EvoSim;
using Microsoft.Xna.Framework;
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

        private static string FormatStatisticsInfo(string stringPrefix, GameTime time, Simulation sim, long Iteration, long creaturesUpdateCycles, long graveYardSize, int saveCount)
        {
            string statisticsInfo = string.Format(
                "{0} {1:dd\\d\\ hh\\:mm\\:ss}:\n" +
                "Elapsed Simulation time {2:dd\\d\\ hh\\:mm\\:ss}.\n" +
                "{3:n0} Simulation Iterations ({4:#,000.00}/s).\n" +
                "{5:n0} creature updates ({6:#,000.00}/s).\n" +
                "{7:n0} Creatures lived their live and are put on the graveyard.\n" +
                "Saved {8} times.\n",
                stringPrefix,
                time.TotalGameTime,
                sim.TotalElapsedSimulationTime,
                Iteration,
                Iteration / time.TotalGameTime.TotalSeconds,
                creaturesUpdateCycles,
                creaturesUpdateCycles / time.TotalGameTime.TotalSeconds,
                graveYardSize + sim.CreatureManager.Graveyard.Count,
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
            GameTime time = new GameTime();
            long creaturesUpdateCycles = 0;
            int saveCount = 0;
            Console.CancelKeyPress += Console_CancelKeyPress;
            Console.CursorVisible = false;
            long graveYardSize = 0;
            while (keepRunnig)
            {
                DateTime now = DateTime.UtcNow;
                time.ElapsedGameTime = now - lastUpdate;
                lastUpdate = DateTime.UtcNow;
                time.TotalGameTime = now - startTime;
                creaturesUpdateCycles += sim.CreatureManager.Creatures.Count;
                sim.NotifyTick(time);


                Iteration++;

                // Update console text once per second (console updates are super slow)
                if ((now - lastConsoleUpdate).TotalSeconds >= 1 && keepRunnig && !Console.IsOutputRedirected)
                {
                    lastConsoleUpdate = DateTime.UtcNow;
                    Console.SetCursorPosition(0,0);
                    string statisticsInfo = FormatStatisticsInfo("Running simulation for", time, sim, Iteration, creaturesUpdateCycles, graveYardSize, saveCount);
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
            string finalInfo = FormatStatisticsInfo("Ran simulation for", time, sim, Iteration, creaturesUpdateCycles, graveYardSize, saveCount);
            Console.WriteLine(finalInfo);


        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            keepRunnig = false;
            Console.WriteLine("Cancel Request, waiting for simulation finish");
            e.Cancel = true;
        }
    }
}
