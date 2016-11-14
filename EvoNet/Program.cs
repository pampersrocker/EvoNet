using EvoNet.AI;
using EvoNet.Objects;
using EvoSim;
using EvoSim.Serialization;
using Microsoft.Xna.Framework;
using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace EvoNet
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //if (!File.Exists("EvoDatabase.sqlite"))
            //{
            //    SQLiteConnection.CreateFile("EvoDatabase.sqlite");
            //}
            SQLiteConnection connection = new SQLiteConnection("Data Source=./EvoDatabase.sqlite");
            //connection.
            CreatureModel model = new CreatureModel(connection);
            CreatureManager manager = new CreatureManager();
            Simulation sim = new Simulation();
            sim.Initialize(sim);
            model.Creatures.Add(new Objects.Creature(Vector2.Zero, 0, sim.CreatureManager));
            model.SaveChanges();
            var creatures = from creature in model.Creatures select creature;
            int count = creatures.Count();
            using (var game = new EvoGame())
                game.Run();
        }
    }
}
