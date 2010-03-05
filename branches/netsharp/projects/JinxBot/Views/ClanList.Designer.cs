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
            this.lbClanMembers = new JinxBot.Views.Chat.CustomDrawnListBox();
            this.SuspendLayout();
            // 
            // lbClanMembers
            // 
            this.lbClanMembers.BackColor = System.Drawing.Color.Black;
            this.lbClanMembers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbClanMembers.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.lbClanMembers.ForeColor = System.Drawing.Color.White;
            this.lbClanMembers.FormattingEnabled = true;
            this.lbClanMembers.Location = new System.Drawing.Point(0, 0);
            this.lbClanMembers.Name = "lbClanMembers";
            this.lbClanMembers.Size = new System.Drawing.Size(274, 472);
            this.lbClanMembers.TabIndex = 0;
            // 
            // ClanList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(274, 472);
            this.Controls.Add(this.lbClanMembers);
            this.Name = "ClanList";
            this.TabText = "Clan List";
            this.Text = "Clan List";
            this.ResumeLayout(false);

        }

        #endregion

        private JinxBot.Views.Chat.CustomDrawnListBox lbClanMembers;

    }
}