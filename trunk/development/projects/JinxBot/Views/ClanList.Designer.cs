namespace JinxBot.Views
{
    partial class ClanList
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
            this.lvClanMembers = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // lvClanMembers
            // 
            this.lvClanMembers.BackColor = System.Drawing.Color.Black;
            this.lvClanMembers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvClanMembers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvClanMembers.ForeColor = System.Drawing.SystemColors.Window;
            this.lvClanMembers.Location = new System.Drawing.Point(0, 0);
            this.lvClanMembers.Name = "lvClanMembers";
            this.lvClanMembers.Size = new System.Drawing.Size(274, 472);
            this.lvClanMembers.TabIndex = 0;
            this.lvClanMembers.UseCompatibleStateImageBehavior = false;
            this.lvClanMembers.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 218;
            // 
            // ClanList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(274, 472);
            this.Controls.Add(this.lvClanMembers);
            this.Name = "ClanList";
            this.TabText = "Clan List";
            this.Text = "Clan List";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvClanMembers;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}