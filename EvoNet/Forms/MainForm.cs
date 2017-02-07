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
using EvoNet.Objects;

namespace EvoNet.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            numberCreaturesAlive.Color = Color.Red;
            foodValueList.Color = Color.Green;
            NumberOfCreaturesAliveGraph.Add("Creatures", numberCreaturesAlive);
            NumberOfCreaturesAliveGraph.Add("Food", foodValueList);
            evoSimControl1.OnUpdate += EvoSimControl1_OnUpdate;

        }

        private void EvoSimControl1_OnUpdate(Microsoft.Xna.Framework.GameTime obj)
        {
            string status = "#: {0} D: {1} max(G): {2} Y: {3} LS: {4} LSA: {5} AvgDA: {6}";
            status = string.Format(
                status,
                CreatureManager.Creatures.Count,
                CreatureManager.numberOfDeaths,
                CreatureManager.MaxGeneration,
                (CreatureManager.Tick * CreatureManager.FixedUpdateTime).ToString("0.00"),
                (Creature.oldestCreatureEver != null ?
                    Creature.oldestCreatureEver.Age :
                    0).ToString("0.00"),
                (CreatureManager.OldestCreatureAlive != null ?
                    CreatureManager.OldestCreatureAlive.Age :
                    0).ToString("0.00"),
                CreatureManager.CalculateAverageAgeOfLastDeadCreatures());
            toolStripStatusLabel1.Text = status;
        }

        GraphValueList foodValueList = new GraphValueList();
        GraphValueList numberCreaturesAlive = new GraphValueList();
        int lastFoodIndex = 0;
        int lastCreatureIndex = 0;

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

        private CreatureManager CreatureManager
        {
            get
            {
                return evoSimControl1.sim.CreatureManager;
            }
        }

        DateTime fictionalDateForFood = DateTime.Now;
        DateTime fictionalDateForCreatures = DateTime.Now;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (lastFoodIndex == 0)
            {
                lastFoodIndex = (int)Math.Max(0, TileMap.FoodRecord.Count - evoSimControl1.gameConfiguration.GraphCount);
            }
            if (lastCreatureIndex == 0)
            {
                lastCreatureIndex = (int)Math.Max(0, CreatureManager.AliveCreaturesRecord.Count - evoSimControl1.gameConfiguration.GraphCount);
            }
            while (lastFoodIndex < TileMap.FoodRecord.Count)
            {
                fictionalDateForFood += TimeSpan.FromSeconds(TileMap.FixedUpdateTime);
                float Value = TileMap.FoodRecord[lastFoodIndex];
                lastFoodIndex++;
                foodValueList.Add(new GraphTimeDoubleValue(fictionalDateForFood, Value));
            }
            while (lastCreatureIndex < CreatureManager.AliveCreaturesRecord.Count)
            {
                fictionalDateForCreatures += TimeSpan.FromSeconds(CreatureManager.FixedUpdateTime);
                float Value = CreatureManager.AliveCreaturesRecord[lastCreatureIndex];
                lastCreatureIndex++;
                numberCreaturesAlive.Add(new GraphTimeDoubleValue(fictionalDateForCreatures, Value));
            }
            if (foodValueList.Count > evoSimControl1.gameConfiguration.GraphCount)
            {
                foodValueList.RemoveRange(0, (int)(foodValueList.Count - evoSimControl1.gameConfiguration.GraphCount));
            }
            if (numberCreaturesAlive.Count > evoSimControl1.gameConfiguration.GraphCount)
            {
                numberCreaturesAlive.RemoveRange(0, (int)(numberCreaturesAlive.Count - evoSimControl1.gameConfiguration.GraphCount));
            }
            //FoodGraph.Refresh();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            evoSimControl1.Serialize(true, true);
        }

        private void showStatisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
            showStatisticsToolStripMenuItem.Checked = !splitContainer1.Panel2Collapsed;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            networkRenderControl1.Simulation = evoSimControl1.sim;
        }

        private void evoSimControl1_MouseClick(object sender, MouseEventArgs e)
        {
            evoSimControl1.EvoSimControl_MouseClick(sender, e);
        }
    }
}
