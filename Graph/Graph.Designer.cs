namespace Graph
{
	partial class Graph
	{
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Komponenten-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung. 
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.pictureBoxGraph = new System.Windows.Forms.PictureBox();
			this.splitContainerMain = new System.Windows.Forms.SplitContainer();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxGraph)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
			this.splitContainerMain.Panel1.SuspendLayout();
			this.splitContainerMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// pictureBoxGraph
			// 
			this.pictureBoxGraph.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBoxGraph.Location = new System.Drawing.Point(0, 0);
			this.pictureBoxGraph.Name = "pictureBoxGraph";
			this.pictureBoxGraph.Size = new System.Drawing.Size(429, 215);
			this.pictureBoxGraph.TabIndex = 0;
			this.pictureBoxGraph.TabStop = false;
			// 
			// splitContainerMain
			// 
			this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
			this.splitContainerMain.Margin = new System.Windows.Forms.Padding(10);
			this.splitContainerMain.Name = "splitContainerMain";
			this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerMain.Panel1
			// 
			this.splitContainerMain.Panel1.Controls.Add(this.pictureBoxGraph);
			this.splitContainerMain.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			// 
			// splitContainerMain.Panel2
			// 
			this.splitContainerMain.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.splitContainerMain.Panel2Collapsed = true;
			this.splitContainerMain.Size = new System.Drawing.Size(429, 215);
			this.splitContainerMain.SplitterDistance = 150;
			this.splitContainerMain.TabIndex = 1;
			// 
			// Graph
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainerMain);
			this.Name = "Graph";
			this.Size = new System.Drawing.Size(429, 215);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxGraph)).EndInit();
			this.splitContainerMain.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
			this.splitContainerMain.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBoxGraph;
		private System.Windows.Forms.SplitContainer splitContainerMain;
	}
}
