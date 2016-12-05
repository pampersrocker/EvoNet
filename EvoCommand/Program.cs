using EvoSim;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EvoCommand {
    class Program {
        static void Main(string[] args) {
            Simulation sim = new Simulation();

            sim.Initialize(sim);

            DateTime startTime = DateTime.UtcNow;
            DateTime lastUpdate;
            DateTime lastSerializationTime = DateTime.UtcNow;
            long i = 0;
            while (true) {
                lastUpdate = DateTime.UtcNow;
                update(new GameTime(DateTime.UtcNow - startTime, lastUpdate - lastUpdate));


                i++;

                if (i % 1000000 == 1)
                    Console.WriteLine("Programm still active - " + DateTime.Now.ToString());

                // Save progress every minute
                if ((DateTime.UtcNow - lastSerializationTime).TotalSeconds > 10) {
                    lastSerializationTime = DateTime.UtcNow;
                    sim.TileMap.SerializeToFile("tilemap.dat");
                    sim.CreatureManager.Serialize("creatures.dat");
                    Console.WriteLine("Save everything.");
                }

            }
        }

        public static void update(GameTime gametime) {

        }
    }
}
