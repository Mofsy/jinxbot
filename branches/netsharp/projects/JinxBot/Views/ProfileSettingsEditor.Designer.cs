namespace JinxBot.Views
{
    partial class ProfileSettingsEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pg = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // pg
            // 
            this.pg.Location = new System.Drawing.Point(0, 3);
            this.pg.Name = "pg";
            this.pg.Size = new System.Drawing.Size(440, 311);
            this.pg.TabIndex = 0;
            // 
            // ProfileSettingsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pg);
            this.Name = "ProfileSettingsEditor";
            this.Size = new System.Drawing.Size(440, 317);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid pg;
    }
}
