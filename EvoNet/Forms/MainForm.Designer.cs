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
            this.evoSimControl1 = new EvoNet.Controls.EvoSimControl();
            this.SuspendLayout();
            // 
            // evoSimControl1
            // 
            this.evoSimControl1.Activated = true;
            this.evoSimControl1.Content = null;
            this.evoSimControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.evoSimControl1.IgnoreFocus = false;
            this.evoSimControl1.Location = new System.Drawing.Point(0, 0);
            this.evoSimControl1.Name = "evoSimControl1";
            this.evoSimControl1.Size = new System.Drawing.Size(284, 261);
            this.evoSimControl1.TabIndex = 0;
            this.evoSimControl1.Text = "evoSimControl1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.evoSimControl1);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.EvoSimControl evoSimControl1;
    }
}
