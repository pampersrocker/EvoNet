using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EvoNet.Controls;
using Graph;
using EvoNet.Map;

namespace EvoNet.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            foodValueList.Color = Color.Green;
            FoodGraph.Add("Food", foodValueList);

            evoSimControl1.OnUpdate += EvoSimControl1_OnUpdate;

        }

        private void EvoSimControl1_OnUpdate(Microsoft.Xna.Framework.GameTime obj)
        {

        }

        GraphValueList foodValueList = new GraphValueList();
        int lastFoodIndex = 0;

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Close();

        }

        private TileMap TileMap
        {
            get
            {
                return evoSimControl1.sim.TileMap;
            }
        }

        DateTime fictionalDate = DateTime.Now;

        private void timer1_Tick(object sender, EventArgs e)
        {
            while (lastFoodIndex < TileMap.FoodRecord.Count)
            {
                fictionalDate += TimeSpan.FromSeconds(TileMap.FixedUpdateTime);
                float Value = TileMap.FoodRecord[lastFoodIndex];
                lastFoodIndex++;
                foodValueList.Add(new GraphTimeDoubleValue(fictionalDate, Value));
            }
            FoodGraph.Refresh();
        }
    }
}
