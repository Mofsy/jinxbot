namespace JinxBot.Views
{
    partial class ChannelSelect
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
            this.SuspendLayout();
            // 
            // ChannelSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(272, 575);
            this.DockAreas = ((JinxBot.Controls.Docking.DockAreas)((JinxBot.Controls.Docking.DockAreas.Float | JinxBot.Controls.Docking.DockAreas.DockLeft)));
            this.Name = "ChannelSelect";
            this.TabText = "Available Channels";
            this.Text = "Available Channels";
            this.ResumeLayout(false);

        }

        #endregion
    }
}