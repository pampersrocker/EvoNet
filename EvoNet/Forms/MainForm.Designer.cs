namespace EvoNet.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showStatisticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.graphsTab = new System.Windows.Forms.TabPage();
            this.networkTab = new System.Windows.Forms.TabPage();
            this.evoSimControl1 = new EvoNet.Controls.EvoSimControl();
            this.NumberOfCreaturesAliveGraph = new EvoNet.Controls.GraphControl();
            this.networkRenderControl1 = new EvoNet.Controls.NetworkRenderControl();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.graphsTab.SuspendLayout();
            this.networkTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem1,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(932, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem1
            // 
            this.fileToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem1});
            this.fileToolStripMenuItem1.Name = "fileToolStripMenuItem1";
            this.fileToolStripMenuItem1.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem1.Text = "&File";
            // 
            // exitToolStripMenuItem1
            // 
            this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
            this.exitToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem1.Size = new System.Drawing.Size(134, 22);
            this.exitToolStripMenuItem1.Text = "E&xit";
            this.exitToolStripMenuItem1.Click += new System.EventHandler(this.exitToolStripMenuItem1_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showStatisticsToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // showStatisticsToolStripMenuItem
            // 
            this.showStatisticsToolStripMenuItem.Checked = true;
            this.showStatisticsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showStatisticsToolStripMenuItem.Name = "showStatisticsToolStripMenuItem";
            this.showStatisticsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.showStatisticsToolStripMenuItem.Text = "&Show Statistics";
            this.showStatisticsToolStripMenuItem.Click += new System.EventHandler(this.showStatisticsToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 588);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(932, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.evoSimControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(932, 564);
            this.splitContainer1.SplitterDistance = 697;
            this.splitContainer1.TabIndex = 5;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.graphsTab);
            this.tabControl1.Controls.Add(this.networkTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(231, 564);
            this.tabControl1.TabIndex = 4;
            // 
            // graphsTab
            // 
            this.graphsTab.Controls.Add(this.NumberOfCreaturesAliveGraph);
            this.graphsTab.Location = new System.Drawing.Point(4, 22);
            this.graphsTab.Name = "graphsTab";
            this.graphsTab.Padding = new System.Windows.Forms.Padding(3);
            this.graphsTab.Size = new System.Drawing.Size(223, 538);
            this.graphsTab.TabIndex = 0;
            this.graphsTab.Text = "Graphs";
            this.graphsTab.UseVisualStyleBackColor = true;
            // 
            // networkTab
            // 
            this.networkTab.Controls.Add(this.networkRenderControl1);
            this.networkTab.Location = new System.Drawing.Point(4, 22);
            this.networkTab.Name = "networkTab";
            this.networkTab.Padding = new System.Windows.Forms.Padding(3);
            this.networkTab.Size = new System.Drawing.Size(223, 538);
            this.networkTab.TabIndex = 1;
            this.networkTab.Text = "Network";
            this.networkTab.UseVisualStyleBackColor = true;
            // 
            // evoSimControl1
            // 
            this.evoSimControl1.Activated = true;
            this.evoSimControl1.Content = null;
            this.evoSimControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.evoSimControl1.IgnoreFocus = true;
            this.evoSimControl1.Location = new System.Drawing.Point(0, 0);
            this.evoSimControl1.Name = "evoSimControl1";
            this.evoSimControl1.Size = new System.Drawing.Size(697, 564);
            this.evoSimControl1.TabIndex = 2;
            this.evoSimControl1.Text = "evoSimControl1";
            // 
            // NumberOfCreaturesAliveGraph
            // 
            this.NumberOfCreaturesAliveGraph.Activated = true;
            this.NumberOfCreaturesAliveGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NumberOfCreaturesAliveGraph.IgnoreFocus = true;
            this.NumberOfCreaturesAliveGraph.Location = new System.Drawing.Point(3, 3);
            this.NumberOfCreaturesAliveGraph.Name = "NumberOfCreaturesAliveGraph";
            this.NumberOfCreaturesAliveGraph.Size = new System.Drawing.Size(217, 532);
            this.NumberOfCreaturesAliveGraph.TabIndex = 3;
            this.NumberOfCreaturesAliveGraph.Text = "graphControl1";
            // 
            // networkRenderControl1
            // 
            this.networkRenderControl1.Activated = true;
            this.networkRenderControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.networkRenderControl1.IgnoreFocus = true;
            this.networkRenderControl1.Location = new System.Drawing.Point(3, 3);
            this.networkRenderControl1.Name = "networkRenderControl1";
            this.networkRenderControl1.Simulation = null;
            this.networkRenderControl1.Size = new System.Drawing.Size(217, 532);
            this.networkRenderControl1.TabIndex = 0;
            this.networkRenderControl1.Text = "networkRenderControl1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(932, 610);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "EvoNet";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.graphsTab.ResumeLayout(false);
            this.networkTab.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Controls.EvoSimControl evoSimControl1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private Controls.GraphControl NumberOfCreaturesAliveGraph;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage graphsTab;
        private System.Windows.Forms.TabPage networkTab;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showStatisticsToolStripMenuItem;
        private Controls.NetworkRenderControl networkRenderControl1;
    }
}
