using EvoNet.AI;
using EvoNet.Forms;
using System;
using System.Windows.Forms;

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
            using (var myForm = new MainForm())
            {
                Application.Run(myForm);
            }
            //using (var game = new EvoGame())
            //    game.Run();
        }
    }
}
